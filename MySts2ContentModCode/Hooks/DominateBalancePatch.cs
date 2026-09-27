using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace MySts2ContentMod.MySts2ContentModCode.Hooks;

/// <summary>
/// 平衡调整:主宰(Dominate)易伤由 1 层提高为 2 层(强化前后均为 2 层),
/// 升级效果由「易伤 +1」改为「费用 1→0」。
/// 原效果:1 费,给予 1 层易伤(强化后 2 层),获得等于目标易伤层数的力量,消耗。
/// 新效果:1 费,给予 2 层易伤,获得等于目标易伤层数的力量,消耗;强化后费用降为 0。
/// 力量数值不受影响,不设上限。
/// </summary>
[HarmonyPatch(typeof(Dominate), "CanonicalVars", MethodType.Getter)]
public static class DominateVarsPatch
{
    public static IEnumerable<DynamicVar> Postfix(IEnumerable<DynamicVar> __result)
    {
        return __result.Select(v => v is PowerVar<VulnerablePower> vulnerable ? new PowerVar<VulnerablePower>(2m) : v);
    }
}

[HarmonyPatch(typeof(Dominate), "OnUpgrade")]
public static class DominateOnUpgradePatch
{
    public static bool Prefix(Dominate __instance)
    {
        __instance.EnergyCost.UpgradeBy(-1);
        return false;
    }
}