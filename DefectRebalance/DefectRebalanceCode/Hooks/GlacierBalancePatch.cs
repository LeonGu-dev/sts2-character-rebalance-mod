using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;

namespace DefectRebalance.DefectRebalanceCode.Hooks;

/// <summary>
/// 平衡调整:冰川(Glacier)格挡由 6/9 提高为 7/10。
/// 原效果:获得 6 格挡(升级+3=9),充 2 颗冰霜球。
/// 新效果:获得 7 格挡(升级仍+3=10),充球不变。
/// </summary>
[HarmonyPatch(typeof(Glacier), "CanonicalVars", MethodType.Getter)]
public static class GlacierBalancePatch
{
    public static IEnumerable<DynamicVar> Postfix(IEnumerable<DynamicVar> __result)
    {
        return __result.Select(v => v is BlockVar b ? new BlockVar(7m, b.Props) : v);
    }
}
