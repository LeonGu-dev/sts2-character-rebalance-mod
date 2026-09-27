using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Orbs;

namespace DefectRebalance.DefectRebalanceCode.Hooks;

/// <summary>
/// 平衡调整:玻璃工艺(Glasswork)。
/// 原效果:获得5(升8)格挡 + 充1颗玻璃球。
/// 新效果:移除格挡;充1颗玻璃球,并为所有玻璃球计数+1(升级+2)。
/// 玻璃球计数为私有字段 _passiveVal,用 AccessTools 反射修改。
/// </summary>
[HarmonyPatch(typeof(Glasswork), "CanonicalVars", MethodType.Getter)]
public static class GlassworkVarsPatch
{
    public static IEnumerable<DynamicVar> Postfix(IEnumerable<DynamicVar> __result)
        => Enumerable.Empty<DynamicVar>();
}

[HarmonyPatch(typeof(Glasswork), "GainsBlock", MethodType.Getter)]
public static class GlassworkGainsBlockPatch
{
    public static bool Postfix(bool __result) => false;
}

[HarmonyPatch(typeof(Glasswork), "OnUpgrade")]
public static class GlassworkOnUpgradePatch
{
    public static bool Prefix() => false;
}

[HarmonyPatch(typeof(Glasswork), "OnPlay")]
public static class GlassworkOnPlayPatch
{
    public static bool Prefix() => false;

    public static async Task Postfix(Task __result, Glasswork __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (__result != null)
        {
            await __result;
        }
        await CreatureCmd.TriggerAnim(__instance.Owner.Creature, "Cast", __instance.Owner.Character.CastAnimDelay);
        await OrbCmd.Channel<GlassOrb>(choiceContext, __instance.Owner);

        decimal amount = __instance.IsUpgraded ? 2m : 1m;
        FieldInfo? counter = AccessTools.Field(typeof(GlassOrb), "_passiveVal");
        foreach (GlassOrb orb in __instance!.Owner!.PlayerCombatState!.OrbQueue.Orbs.OfType<GlassOrb>())
        {
            counter!.SetValue(orb, (decimal)(counter.GetValue(orb) ?? 0m) + amount);
        }
    }
}
