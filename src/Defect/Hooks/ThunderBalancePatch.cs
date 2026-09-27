using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace DefectRebalance.DefectRebalanceCode.Hooks;

/// <summary>
/// 平衡调整:雷霆(Thunder)能力值由 8/11 降低为 6/8。
/// 原效果:ThunderPower 8,升级+3=11(激发闪电球后造成该值伤害)。
/// 新效果:ThunderPower 6,升级增量改为+2,强化后=8。
/// 说明:当前游戏实际为 8/11(用户初稿写 6/8 有误,已核对 ilspycmd)。
/// </summary>
[HarmonyPatch(typeof(Thunder), "CanonicalVars", MethodType.Getter)]
public static class ThunderBalancePatch
{
    public static IEnumerable<DynamicVar> Postfix(IEnumerable<DynamicVar> __result)
    {
        return __result.Select(v => v is PowerVar<ThunderPower> ? new PowerVar<ThunderPower>(6m) : v);
    }
}

/// <summary>
/// 雷霆升级:原 ThunderPower +3,改为 +2(使强化后为 8)。
/// </summary>
[HarmonyPatch(typeof(Thunder), "OnUpgrade")]
public static class ThunderOnUpgradePatch
{
    public static bool Prefix(Thunder __instance)
    {
        __instance.DynamicVars["ThunderPower"].UpgradeValueBy(2m);
        return false;
    }
}
