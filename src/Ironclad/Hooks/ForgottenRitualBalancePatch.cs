using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace MySts2ContentMod.MySts2ContentModCode.Hooks;

/// <summary>
/// 平衡调整(改类型):被遗忘的仪式(ForgottenRitual)。
/// 原:1费技能,获得3能量(升级+1)。
/// 新:改为1费攻击牌,对任一敌人造成8点伤害(升级11);
///    若本回合消耗过牌,则获得3点能量(升级4)。消耗。
/// 由于 Type / TargetType 是继承自 CardModel 的 virtual 属性且本卡未 override,
/// 需在基类 getter 上 patch 并按实例分派(同 CardRarityAdjustmentsPatch 做法)。
/// 注意:Harmony 此版本不支持 async prefix,故用「同步 skip prefix + async postfix」替换 OnPlay。
/// </summary>
[HarmonyPatch(typeof(CardModel), "Type", MethodType.Getter)]
public static class ForgottenRitualTypePatch
{
    public static CardType Postfix(CardType __result, CardModel __instance)
    {
        return __instance is ForgottenRitual ? CardType.Attack : __result;
    }
}

[HarmonyPatch(typeof(CardModel), "TargetType", MethodType.Getter)]
public static class ForgottenRitualTargetPatch
{
    public static TargetType Postfix(TargetType __result, CardModel __instance)
    {
        return __instance is ForgottenRitual ? TargetType.AnyEnemy : __result;
    }
}

/// <summary>
/// 被遗忘的仪式(ForgottenRitual):本回合消耗过牌时金色高亮。
/// 参考"契约终结/眼部攻击"模式:只覆盖 ShouldGlowGoldInternal(条件满足时发光),
/// 不改 IsPlayable(卡牌始终可打出)。
/// </summary>
[HarmonyPatch(typeof(CardModel), "ShouldGlowGoldInternal", MethodType.Getter)]
public static class ForgottenRitualGlowPatch
{
    public static bool Postfix(bool __result, CardModel __instance)
    {
        if (__instance is ForgottenRitual)
        {
            return ForgottenRitualBalanceHelper.WasCardExhaustedThisTurn(__instance);
        }
        return __result;
    }
}

internal static class ForgottenRitualBalanceHelper
{
    public static bool WasCardExhaustedThisTurn(CardModel card)
    {
        return CombatManager.Instance.History.Entries
            .OfType<CardExhaustedEntry>()
            .Any(e => e.HappenedThisTurn(card.CombatState) && e.Actor == card.Owner.Creature);
    }
}

/// <summary>
/// 被遗忘的仪式(ForgottenRitual)数值:改为 8 伤害 + 3 能量。
/// </summary>
[HarmonyPatch(typeof(ForgottenRitual), "CanonicalVars", MethodType.Getter)]
public static class ForgottenRitualVarsPatch
{
    public static IEnumerable<DynamicVar> Postfix(IEnumerable<DynamicVar> __result)
    {
        return new DynamicVar[]
        {
            new DamageVar(8m, ValueProp.Move),
            new EnergyVar(3)
        };
    }
}

/// <summary>
/// 被遗忘的仪式(ForgottenRitual)升级:伤害 8→11,能量 3→4。
/// </summary>
[HarmonyPatch(typeof(ForgottenRitual), "OnUpgrade")]
public static class ForgottenRitualUpgradePatch
{
    public static bool Prefix(ForgottenRitual __instance)
    {
        __instance.DynamicVars.Damage.UpgradeValueBy(3m);
        __instance.DynamicVars.Energy.UpgradeValueBy(1m);
        return false;
    }
}

/// <summary>
/// 被遗忘的仪式(ForgottenRitual)OnPlay 替换:造成伤害;若本回合消耗过牌则获得能量。
/// </summary>
[HarmonyPatch(typeof(ForgottenRitual), "OnPlay")]
public static class ForgottenRitualOnPlayPatch
{
    public static bool Prefix()
    {
        return false;
    }

    public static async Task Postfix(Task __result, ForgottenRitual __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (__result != null)
        {
            await __result;
        }

        await DamageCmd.Attack(__instance.DynamicVars.Damage.BaseValue)
            .FromCard(__instance, cardPlay)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        if (ForgottenRitualBalanceHelper.WasCardExhaustedThisTurn(__instance))
        {
            await PlayerCmd.GainEnergy(__instance.DynamicVars.Energy.IntValue, __instance.Owner);
        }
    }
}