using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Orbs;

namespace DefectRebalance.DefectRebalanceCode.Hooks;

/// <summary>
/// 平衡调整:漆黑(Darkness)。
/// 原效果:充1颗黑暗球,让场上所有黑暗球被动触发1次(升级后2次)。
/// 新效果:被动恒触发1次(升级不再2次)。
/// 实现:重写 OnPlay(跳过原实现,充球后对每颗黑暗球只触发1次被动)。
/// </summary>
[HarmonyPatch(typeof(Darkness), "OnPlay")]
public static class DarknessOnPlayPatch
{
    public static bool Prefix() => false;

    public static async Task Postfix(Task __result, Darkness __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (__result != null)
        {
            await __result;
        }
        await CreatureCmd.TriggerAnim(__instance!.Owner!.Creature, "Cast", __instance.Owner.Character.CastAnimDelay);
        await OrbCmd.Channel<DarkOrb>(choiceContext, __instance.Owner);
        foreach (OrbModel orb in __instance!.Owner!.PlayerCombatState!.OrbQueue.Orbs.Where(o => o is DarkOrb).ToList())
        {
            await OrbCmd.Passive(choiceContext, orb, null);
        }
    }
}

/// <summary>
/// 漆黑:未强化获得【消耗】关键词;强化后去掉消耗(IsUpgraded 为真时不加 Exhaust)。
/// Darkness 未重写 CanonicalKeywords,因此 patch 基类 getter;升级时还要显式清除
/// LocalKeywords 中可能已经缓存的消耗关键词。
/// </summary>
[HarmonyPatch(typeof(CardModel), "CanonicalKeywords", MethodType.Getter)]
public static class DarknessExhaustPatch
{
    public static IEnumerable<CardKeyword> Postfix(IEnumerable<CardKeyword> __result, CardModel __instance)
        => __instance is Darkness { IsUpgraded: false }
            ? new[] { CardKeyword.Exhaust }.Concat(__result ?? Enumerable.Empty<CardKeyword>())
            : __result;
}

[HarmonyPatch(typeof(CardModel), "OnUpgrade")]
public static class DarknessUpgradePatch
{
    public static bool Prefix(CardModel __instance)
    {
        if (__instance is not Darkness)
        {
            return true;
        }
        __instance.RemoveKeyword(CardKeyword.Exhaust);
        return false;
    }
}
