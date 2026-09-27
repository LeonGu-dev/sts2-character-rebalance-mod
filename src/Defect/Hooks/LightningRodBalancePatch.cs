using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace DefectRebalance.DefectRebalanceCode.Hooks;

/// <summary>
/// 平衡调整:引雷针(LightningRod)重做。
/// 原:1费普通技能,获得4(升7)格挡+能力(每回合充1颗闪电球,可叠2次)。
/// 新:1费普通能力牌,打出时激发所有闪电充能球(闪电正常Evoke伤害照常),每激发一颗获得2(升3)点荆棘。
/// </summary>

[HarmonyPatch(typeof(CardModel), "Type", MethodType.Getter)]
public static class LightningRodTypePatch
{
    public static CardType Postfix(CardType __result, CardModel __instance)
        => __instance is LightningRod ? CardType.Power : __result;
}

[HarmonyPatch(typeof(CardModel), "Rarity", MethodType.Getter)]
public static class LightningRodRarityPatch
{
    public static CardRarity Postfix(CardRarity __result, CardModel __instance)
        => __instance is LightningRod ? CardRarity.Uncommon : __result;
}

[HarmonyPatch(typeof(LightningRod), "GainsBlock", MethodType.Getter)]
public static class LightningRodGainsBlockPatch
{
    public static bool Postfix(bool __result) => false;
}

[HarmonyPatch(typeof(LightningRod), "CanonicalVars", MethodType.Getter)]
public static class LightningRodVarsPatch
{
    public static IEnumerable<DynamicVar> Postfix(IEnumerable<DynamicVar> __result)
        => new DynamicVar[] { new PowerVar<ThornsPower>(2m) };
}

[HarmonyPatch(typeof(LightningRod), "OnUpgrade")]
public static class LightningRodUpgradePatch
{
    public static bool Prefix(LightningRod __instance)
    {
        __instance.DynamicVars["ThornsPower"].UpgradeValueBy(1m);
        return false;
    }
}

[HarmonyPatch(typeof(LightningRod), "OnPlay")]
public static class LightningRodOnPlayPatch
{
    public static bool Prefix() => false;

    public static async Task Postfix(Task __result, LightningRod __instance, PlayerChoiceContext choiceContext)
    {
        if (__result != null)
        {
            await __result;
        }
        var owner = __instance.Owner!;
        var queue = owner.PlayerCombatState!.OrbQueue;
        decimal thorns = __instance.DynamicVars["ThornsPower"].BaseValue;
        foreach (OrbModel orb in queue.Orbs.OfType<LightningOrb>().ToList())
        {
            queue.Remove(orb);
            NCombatRoom.Instance?.GetCreatureNode(owner.Creature)?.OrbManager?.EvokeOrbAnim(orb);
            IEnumerable<Creature> targets = await orb.Evoke(choiceContext);
            await Hook.AfterOrbEvoked(choiceContext, owner.Creature.CombatState!, orb, targets);
            orb.RemoveInternal();
            await PowerCmd.Apply<ThornsPower>(choiceContext, owner.Creature, thorns, owner.Creature, __instance);
        }
    }
}
