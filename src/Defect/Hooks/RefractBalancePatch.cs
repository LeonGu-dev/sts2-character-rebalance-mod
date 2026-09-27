using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DefectRebalance.DefectRebalanceCode.Powers;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Orbs;

namespace DefectRebalance.DefectRebalanceCode.Hooks;

/// <summary>
/// 平衡调整:折射(Refract)重做。
/// 原:3费罕见攻击,10(升13)伤害打2次+充2颗玻璃球。
/// 新:3费稀有能力,将要受到伤害时激发一颗计数不为0的玻璃充能球并阻止此伤害;升级后打出时额外生成1颗玻璃球。
/// </summary>

[HarmonyPatch(typeof(CardModel), "Rarity", MethodType.Getter)]
public static class RefractRarityPatch
{
    public static CardRarity Postfix(CardRarity __result, CardModel __instance)
        => __instance is Refract ? CardRarity.Rare : __result;
}

[HarmonyPatch(typeof(CardModel), "Type", MethodType.Getter)]
public static class RefractTypePatch
{
    public static CardType Postfix(CardType __result, CardModel __instance)
        => __instance is Refract ? CardType.Power : __result;
}

[HarmonyPatch(typeof(CardModel), "TargetType", MethodType.Getter)]
public static class RefractTargetTypePatch
{
    public static TargetType Postfix(TargetType __result, CardModel __instance)
        => __instance is Refract ? TargetType.Self : __result;
}

[HarmonyPatch(typeof(Refract), "CanonicalVars", MethodType.Getter)]
public static class RefractVarsPatch
{
    public static IEnumerable<DynamicVar> Postfix(IEnumerable<DynamicVar> __result)
        => new DynamicVar[] { new PowerVar<RefractReworkPower>(1m) };
}

[HarmonyPatch(typeof(Refract), "OnUpgrade")]
public static class RefractUpgradePatch
{
    public static bool Prefix() => false;
}

[HarmonyPatch(typeof(Refract), "OnPlay")]
public static class RefractOnPlayPatch
{
    public static bool Prefix() => false;

    public static async Task Postfix(Task __result, Refract __instance, PlayerChoiceContext choiceContext)
    {
        if (__result != null)
        {
            await __result;
        }
        await CreatureCmd.TriggerAnim(__instance.Owner!.Creature, "PowerUp", __instance.Owner.Character.PowerUpAnimDelay);
        await PowerCmd.Apply<RefractReworkPower>(choiceContext, __instance.Owner.Creature, 1m, __instance.Owner.Creature, __instance);
        if (__instance.IsUpgraded)
        {
            await OrbCmd.Channel<GlassOrb>(choiceContext, __instance.Owner);
        }
    }
}
