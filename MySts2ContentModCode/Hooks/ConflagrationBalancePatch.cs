using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace MySts2ContentMod.MySts2ContentModCode.Hooks;

/// <summary>
/// 平衡调整(重做):焚烧(Conflagration)。
/// 原:1费攻击,对所有敌人造成 2 伤害×4 次,升级 +1 次。
/// 新:2费攻击(强化前后均 2 费),消耗手牌中所有攻击牌;每消耗一张,
///    对所有敌人造成 3 点伤害(升级 4),并对自身造成 1 点生命损失。
/// 注意:Harmony 此版本不支持 async prefix,故用「同步 skip prefix + async postfix」替换 OnPlay。
/// </summary>
[HarmonyPatch(typeof(Conflagration), "CanonicalVars", MethodType.Getter)]
public static class ConflagrationVarsPatch
{
    public static IEnumerable<DynamicVar> Postfix(IEnumerable<DynamicVar> __result)
    {
        return new DynamicVar[]
        {
            new DamageVar(6m, ValueProp.Move),
            new HpLossVar(1m)
        };
    }
}

/// <summary>
/// 焚烧(Conflagration)费用:原 1 费,改为 2 费(强化前后均 2 费)。
/// 通过 patch 基类 CardModel.CanonicalEnergyCost getter 对焚烧返回固定 2,
/// 覆盖 canonical 模板与可变克隆,统一且不再依赖易叠加的 GetWithModifiers postfix。
/// </summary>
[HarmonyPatch(typeof(CardModel), "CanonicalEnergyCost", MethodType.Getter)]
public static class ConflagrationCostPatch
{
    public static int Postfix(int __result, CardModel __instance)
        => __instance is Conflagration ? 2 : __result;
}

/// <summary>
/// 焚烧(Conflagration)消耗属性:打出后自身进入消耗堆。
/// 通过 patch 基类 CardModel.CanonicalKeywords getter 对焚烧追加 CardKeyword.Exhaust。
/// </summary>
[HarmonyPatch(typeof(CardModel), "CanonicalKeywords", MethodType.Getter)]
public static class ConflagrationExhaustPatch
{
    public static IEnumerable<CardKeyword> Postfix(IEnumerable<CardKeyword> __result, CardModel __instance)
        => __instance is Conflagration
            ? new[] { CardKeyword.Exhaust }.Concat(__result ?? Enumerable.Empty<CardKeyword>())
            : __result;
}

/// <summary>
/// 焚烧(Conflagration)升级:原为 Repeat +1,现改为每张攻击牌的伤害 6→8。
/// </summary>
[HarmonyPatch(typeof(Conflagration), "OnUpgrade")]
public static class ConflagrationUpgradePatch
{
    public static bool Prefix(Conflagration __instance)
    {
        __instance.DynamicVars.Damage.UpgradeValueBy(2m);
        return false;
    }
}

/// <summary>
/// 焚烧(Conflagration)OnPlay 替换:跳过原方法,改为消耗手牌所有攻击牌,
/// 每张对全体伤害并自伤。
/// </summary>
[HarmonyPatch(typeof(Conflagration), "OnPlay")]
public static class ConflagrationOnPlayPatch
{
    public static bool Prefix()
    {
        return false;
    }

    public static async Task Postfix(Task __result, Conflagration __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (__result != null)
        {
            await __result;
        }

        List<CardModel> attackCards = PileType.Hand.GetPile(__instance.Owner).Cards
            .Where(c => c != null && c.Type == CardType.Attack)
            .ToList();

        foreach (CardModel card in attackCards)
        {
            await CardCmd.Exhaust(choiceContext, card);
            await DamageCmd.Attack(__instance.DynamicVars.Damage.BaseValue)
                .FromCard(__instance, cardPlay)
                .TargetingAllOpponents(__instance.CombatState)
                .Execute(choiceContext);
            await CreatureCmd.Damage(choiceContext, __instance.Owner.Creature,
                __instance.DynamicVars.HpLoss.BaseValue,
                ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, __instance, cardPlay);
        }
    }
}