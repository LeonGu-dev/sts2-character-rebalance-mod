using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace RebalanceMod.Regent.Hooks;

// Batch A: keep the original effects and change only their displayed values.
[HarmonyPatch(typeof(CollisionCourse), "CanonicalVars", MethodType.Getter)]
public static class CollisionCourseNumericPatch
{
    public static IEnumerable<DynamicVar> Postfix(IEnumerable<DynamicVar> __result)
        => __result.Select(v => v is DamageVar damage ? new DamageVar(9m, damage.Props) : v);
}

[HarmonyPatch(typeof(CosmicIndifference), "CanonicalVars", MethodType.Getter)]
public static class CosmicIndifferenceNumericPatch
{
    public static IEnumerable<DynamicVar> Postfix(IEnumerable<DynamicVar> __result)
        => __result.Select(v => v is BlockVar block ? new BlockVar(5m, block.Props) : v);
}

[HarmonyPatch(typeof(Parry), "CanonicalVars", MethodType.Getter)]
public static class ParryNumericPatch
{
    public static IEnumerable<DynamicVar> Postfix(IEnumerable<DynamicVar> __result)
        => __result.Select(v => v is PowerVar<ParryPower> ? new PowerVar<ParryPower>(9m) : v);
}

[HarmonyPatch(typeof(ManifestAuthority), "CanonicalVars", MethodType.Getter)]
public static class ManifestAuthorityNumericPatch
{
    public static IEnumerable<DynamicVar> Postfix(IEnumerable<DynamicVar> __result)
        => __result.Select(v => v is BlockVar block ? new BlockVar(6m, block.Props) : v);
}

[HarmonyPatch(typeof(ManifestAuthority), "OnUpgrade")]
public static class ManifestAuthorityUpgradePatch
{
    // The original upgrade only adds one Block; its upgraded colorless card stays in OnPlay.
    public static bool Prefix() => false;
}

[HarmonyPatch(typeof(DyingStar), "CanonicalVars", MethodType.Getter)]
public static class DyingStarNumericPatch
{
    public static IEnumerable<DynamicVar> Postfix(IEnumerable<DynamicVar> __result)
        => __result.Select(v => v switch
        {
            DamageVar damage => new DamageVar(12m, damage.Props),
            { Name: "StrengthLoss" } => new DynamicVar("StrengthLoss", 4m),
            _ => v
        });
}

[HarmonyPatch(typeof(DyingStar), "OnUpgrade")]
public static class DyingStarUpgradePatch
{
    // Vanilla adds 2 damage; the target upgrade is +4 damage and +2 Strength loss.
    public static void Postfix(DyingStar __instance)
        => __instance.DynamicVars.Damage.UpgradeValueBy(2m);
}

[HarmonyPatch(typeof(DecisionsDecisions), "CanonicalVars", MethodType.Getter)]
public static class DecisionsDecisionsNumericPatch
{
    public static IEnumerable<DynamicVar> Postfix(IEnumerable<DynamicVar> __result)
        => __result.Select(v => v is CardsVar ? new CardsVar(2) : v);
}
