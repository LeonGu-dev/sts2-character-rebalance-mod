using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace MySts2ContentMod.MySts2ContentModCode.Hooks;

/// <summary>
/// 平衡调整(改机制):痛殴(Thrash)。
/// 原:1费攻击,造成4伤害×2;随机消耗手牌一张攻击牌并将其伤害添加给这张牌;升级+2伤害。
/// 新:1费攻击,造成8伤害×1;消耗手牌一张攻击牌并将其伤害添加给这张牌;
///    未升级:随机选择;升级后:由玩家自行选择消耗哪张攻击牌。升级不再增加基础伤害。
/// 注意:Harmony 此版本不支持 async prefix,故用「同步 skip prefix + async postfix」替换 OnPlay。
/// </summary>
[HarmonyPatch(typeof(Thrash), "CanonicalVars", MethodType.Getter)]
public static class ThrashVarsPatch
{
    public static IEnumerable<DynamicVar> Postfix(IEnumerable<DynamicVar> __result)
    {
        return __result.Select(v => v is DamageVar damage ? new DamageVar(8m, damage.Props) : v);
    }
}

/// <summary>
/// 痛殴(Thrash)升级:原伤害+2,改为"由玩家选择消耗哪张攻击牌"(即升级不再加伤害)。
/// </summary>
[HarmonyPatch(typeof(Thrash), "OnUpgrade")]
public static class ThrashUpgradePatch
{
    public static bool Prefix()
    {
        return false;
    }
}

/// <summary>
/// 痛殴(Thrash)OnPlay 替换:8伤害×1,消耗一张攻击牌(升级后玩家选择),吸收其伤害。
/// </summary>
[HarmonyPatch(typeof(Thrash), "OnPlay")]
public static class ThrashOnPlayPatch
{
    public static bool Prefix()
    {
        return false;
    }

    public static async Task Postfix(Task __result, Thrash __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (__result != null)
        {
            await __result;
        }

        await DamageCmd.Attack(__instance.DynamicVars.Damage.BaseValue)
            .FromCard(__instance, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_thrash")
            .Execute(choiceContext);

        CardPile handPile = PileType.Hand.GetPile(__instance.Owner);
        IEnumerable<CardModel> attackCards = handPile.Cards.Where(c => c != null && c.Type == CardType.Attack && c != __instance);

        CardModel cardModel;
        if (__instance.IsUpgraded)
        {
            CardSelectorPrefs prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1);
            cardModel = (await CardSelectCmd.FromHand(choiceContext, __instance.Owner, prefs, c => c.Type == CardType.Attack && c != __instance, __instance)).FirstOrDefault();
        }
        else
        {
            cardModel = __instance.Owner.RunState.Rng.CombatCardSelection.NextItem(attackCards);
        }

        if (cardModel != null)
        {
            decimal damage = 0m;
            if (cardModel.DynamicVars.ContainsKey("CalculatedDamage"))
            {
                damage = cardModel.DynamicVars.CalculatedDamage.Calculate(null);
            }
            else if (cardModel.DynamicVars.ContainsKey("Damage"))
            {
                damage = cardModel.DynamicVars.Damage.BaseValue;
            }
            else if (cardModel.DynamicVars.ContainsKey("OstyDamage"))
            {
                damage = cardModel.DynamicVars.OstyDamage.BaseValue;
            }

            damage = Hook.ModifyDamage(__instance.Owner.RunState, __instance.Owner.Creature.CombatState, null,
                __instance.Owner.Creature, damage, ValueProp.Move, cardModel, null,
                ModifyDamageHookType.All, CardPreviewMode.None, out _);

            __instance.DynamicVars.Damage.BaseValue += damage;
            await CardCmd.Exhaust(choiceContext, cardModel);
        }
    }
}