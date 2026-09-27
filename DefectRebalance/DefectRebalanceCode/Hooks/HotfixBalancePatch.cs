using System.Collections.Generic;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Cards;

namespace DefectRebalance.DefectRebalanceCode.Hooks;

/// <summary>
/// 平衡调整:热修复(Hotfix)。
/// 原效果:0费,获得2点临时集中,自带消耗,升级仅去掉消耗(集中仍为2)。
/// 新效果:0费,获得2(升级3)点临时集中,无消耗;每次打出后本回合费用+1。
/// </summary>

[HarmonyPatch(typeof(Hotfix), "CanonicalKeywords", MethodType.Getter)]
public static class HotfixKeywordPatch
{
    public static IEnumerable<CardKeyword> Postfix(IEnumerable<CardKeyword> __result)
        => System.Array.Empty<CardKeyword>();
}

[HarmonyPatch(typeof(Hotfix), "OnUpgrade")]
public static class HotfixOnUpgradePatch
{
    public static bool Prefix(Hotfix __instance)
    {
        __instance.DynamicVars["FocusPower"].UpgradeValueBy(1m);
        return false;
    }
}

[HarmonyPatch(typeof(Hotfix), "OnPlay")]
public static class HotfixOnPlayPatch
{
    public static async Task Postfix(Task __result, Hotfix __instance)
    {
        await __result;
        __instance.EnergyCost.AddThisTurn(1);
    }
}
