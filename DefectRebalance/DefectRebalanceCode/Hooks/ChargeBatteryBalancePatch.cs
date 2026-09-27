using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;

namespace DefectRebalance.DefectRebalanceCode.Hooks;

/// <summary>
/// 平衡调整:充电(ChargeBattery)格挡由 7/10 提高为 8/11。
/// 原效果:获得 7 格挡(升级+3=10),下回合+1能量。
/// 新效果:获得 8 格挡(升级仍+3=11)。
/// 说明:当前游戏实际为 7/10(用户初稿写 7/9 有误,已核对 ilspycmd)。
/// </summary>
[HarmonyPatch(typeof(ChargeBattery), "CanonicalVars", MethodType.Getter)]
public static class ChargeBatteryBalancePatch
{
    public static IEnumerable<DynamicVar> Postfix(IEnumerable<DynamicVar> __result)
    {
        return __result.Select(v => v is BlockVar b ? new BlockVar(8m, b.Props) : v);
    }
}
