using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace RebalanceMod.Regent.Hooks;

[HarmonyPatch(typeof(KnowThyPlace), "OnUpgrade")]
public static class KnowThyPlaceUpgradePatch
{
    public static bool Prefix(KnowThyPlace __instance)
    {
        __instance.DynamicVars.Weak.UpgradeValueBy(1m);
        __instance.DynamicVars.Vulnerable.UpgradeValueBy(1m);
        return false;
    }
}

[HarmonyPatch(typeof(CardModel), "Rarity", MethodType.Getter)]
public static class RegentSimpleRarityPatch
{
    public static CardRarity Postfix(CardRarity __result, CardModel __instance)
        => __instance switch
        {
            Glow => CardRarity.Uncommon,
            Terraforming => CardRarity.Common,
            NeutronAegis => CardRarity.Uncommon,
            _ => __result
        };
}

[HarmonyPatch(typeof(Glow), "CanonicalVars", MethodType.Getter)]
public static class GlowVarsPatch
{
    public static IEnumerable<DynamicVar> Postfix(IEnumerable<DynamicVar> __result)
        => __result.Select(v => v is CardsVar ? new CardsVar(2) : v);
}

[HarmonyPatch(typeof(Glow), "OnPlay")]
public static class GlowPlayPatch
{
    public static bool Prefix(Glow __instance, PlayerChoiceContext choiceContext, ref Task __result)
    {
        __result = Play(__instance, choiceContext);
        return false;
    }

    private static async Task Play(Glow card, PlayerChoiceContext choiceContext)
    {
        await CreatureCmd.TriggerAnim(card.Owner.Creature, "Cast", card.Owner.Character.CastAnimDelay);
        await PlayerCmd.GainStars(card.DynamicVars.Stars.BaseValue, card.Owner);
        await CardPileCmd.Draw(choiceContext, card.DynamicVars.Cards.BaseValue, card.Owner);
        await PowerCmd.Apply<DrawCardsNextTurnPower>(choiceContext, card.Owner.Creature, 1m,
            card.Owner.Creature, card);
    }
}

[HarmonyPatch(typeof(HiddenCache), "CanonicalVars", MethodType.Getter)]
public static class HiddenCacheVarsPatch
{
    public static IEnumerable<DynamicVar> Postfix(IEnumerable<DynamicVar> __result)
        => __result.Select(v => v switch
        {
            StarsVar => new StarsVar(2),
            PowerVar<StarNextTurnPower> => new PowerVar<StarNextTurnPower>(2m),
            _ => v
        });
}

[HarmonyPatch(typeof(Resonance), "CanonicalStarCost", MethodType.Getter)]
public static class ResonanceStarCostPatch
{
    public static int Postfix(int __result) => 1;
}

[HarmonyPatch(typeof(BundleOfJoy), "OnUpgrade")]
public static class BundleOfJoyUpgradePatch
{
    // Its base count of three remains unchanged when upgraded.
    public static bool Prefix() => false;
}

[HarmonyPatch(typeof(BundleOfJoy), "OnPlay")]
public static class BundleOfJoyPlayPatch
{
    public static bool Prefix(BundleOfJoy __instance, ref Task __result)
    {
        __result = Play(__instance);
        return false;
    }

    private static async Task Play(BundleOfJoy card)
    {
        IEnumerable<CardModel> cards = CardFactory.GetDistinctForCombat(card.Owner,
            ModelDb.CardPool<ColorlessCardPool>().GetUnlockedCards(
                card.Owner.UnlockState, card.Owner.RunState.CardMultiplayerConstraint),
            card.DynamicVars.Cards.IntValue, card.Owner.RunState.Rng.CombatCardGeneration);
        foreach (CardModel generated in cards)
        {
            if (card.IsUpgraded)
            {
                CardCmd.Upgrade(generated);
            }
            await CardPileCmd.AddGeneratedCardToCombat(generated, PileType.Hand, card.Owner);
        }
    }
}

[HarmonyPatch(typeof(CardModel), "CanonicalEnergyCost", MethodType.Getter)]
public static class ArsenalEnergyCostPatch
{
    public static int Postfix(int __result, CardModel __instance)
        => __instance is Arsenal ? 2 : __result;
}

[HarmonyPatch(typeof(NeutronAegis), "CanonicalStarCost", MethodType.Getter)]
public static class NeutronAegisStarCostPatch
{
    public static int Postfix(int __result) => 4;
}
