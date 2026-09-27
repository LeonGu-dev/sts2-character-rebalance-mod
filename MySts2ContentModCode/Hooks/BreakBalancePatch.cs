using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace MySts2ContentMod.MySts2ContentModCode.Hooks;

/// <summary>
/// 平衡调整:破击(Break)伤害由 20/30 降低为 15/20。
/// 原效果:20 伤害(强化后 30),给予 5 易伤(强化后 7)。
/// 新效果:15 伤害(强化后 20),给予 5 易伤(强化后 7,不变)。
/// 替换 CanonicalVars 中 DamageVar 基值(20 → 15),并补丁 OnUpgrade 使其伤害 +10 → +5。
/// </summary>
[HarmonyPatch(typeof(Break), "CanonicalVars", MethodType.Getter)]
public static class BreakBalancePatch
{
    public static IEnumerable<DynamicVar> Postfix(IEnumerable<DynamicVar> __result)
    {
        return __result.Select(v => v is DamageVar damage ? new DamageVar(15m, ValueProp.Move) : v);
    }
}

/// <summary>
/// 破击(Break)升级逻辑:原升级为伤害 +10 与易伤 +2,现改为伤害 +5 与易伤 +2。
/// </summary>
[HarmonyPatch(typeof(Break), "OnUpgrade")]
public static class BreakOnUpgradePatch
{
    public static bool Prefix(Break __instance)
    {
        __instance.DynamicVars.Damage.UpgradeValueBy(5m);
        __instance.DynamicVars.Vulnerable.UpgradeValueBy(2m);
        return false;
    }
}