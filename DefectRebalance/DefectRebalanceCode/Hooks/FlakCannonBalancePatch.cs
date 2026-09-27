using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;

namespace DefectRebalance.DefectRebalanceCode.Hooks;

/// <summary>
/// 平衡调整:散射炮(FlakCannon)伤害由 8/11 降低为 7/10。
/// 原效果:伤害 8(升级+3=11);按非消耗堆状态牌数打多段,每段前消耗一张状态牌。
/// 新效果:伤害 7(升级仍+3=10)。
/// 只替换 CanonicalVars 中的 DamageVar 基值;升级逻辑(+3)不变。
/// </summary>
[HarmonyPatch(typeof(FlakCannon), "CanonicalVars", MethodType.Getter)]
public static class FlakCannonBalancePatch
{
    public static IEnumerable<DynamicVar> Postfix(IEnumerable<DynamicVar> __result)
    {
        return __result.Select(v => v is DamageVar d ? new DamageVar(7m, d.Props) : v);
    }
}
