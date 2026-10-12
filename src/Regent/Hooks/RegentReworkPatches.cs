using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace RebalanceMod.Regent.Hooks;

[HarmonyPatch(typeof(CardModel), "CanonicalEnergyCost", MethodType.Getter)]
public static class RegentReworkEnergyCostPatch
{
    public static int Postfix(int __result, CardModel __instance)
        => __instance switch
        {
            GammaBlast => 1,
            ForegoneConclusion => 2,
            _ => __result
        };
}

[HarmonyPatch(typeof(CardModel), "TargetType", MethodType.Getter)]
public static class RegentReworkTargetPatch
{
    public static TargetType Postfix(TargetType __result, CardModel __instance)
        => __instance switch
        {
            GammaBlast => TargetType.AllEnemies,
            Conqueror => TargetType.Self,
            _ => __result
        };
}

[HarmonyPatch(typeof(CardModel), "Type", MethodType.Getter)]
public static class ForegoneConclusionTypePatch
{
    public static CardType Postfix(CardType __result, CardModel __instance)
        => __instance is ForegoneConclusion ? CardType.Power : __result;
}

[HarmonyPatch(typeof(GammaBlast), "CanonicalStarCost", MethodType.Getter)]
public static class GammaBlastStarCostPatch
{
    public static int Postfix(int __result) => 2;
}

[HarmonyPatch(typeof(GammaBlast), "CanonicalVars", MethodType.Getter)]
public static class GammaBlastVarsPatch
{
    public static IEnumerable<DynamicVar> Postfix(IEnumerable<DynamicVar> __result)
        => new DynamicVar[] { new DamageVar(6m, MegaCrit.Sts2.Core.ValueProps.ValueProp.Move) };
}

[HarmonyPatch(typeof(GammaBlast), "OnUpgrade")]
public static class GammaBlastUpgradePatch
{
    public static bool Prefix(GammaBlast __instance)
    {
        __instance.DynamicVars.Damage.UpgradeValueBy(3m);
        return false;
    }
}

[HarmonyPatch(typeof(GammaBlast), "OnPlay")]
public static class GammaBlastPlayPatch
{
    public static bool Prefix(GammaBlast __instance, PlayerChoiceContext choiceContext,
        CardPlay cardPlay, ref Task __result)
    {
        __result = Play(__instance, choiceContext, cardPlay);
        return false;
    }

    private static async Task Play(GammaBlast card, PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        List<CardModel> colorlessCards = PileType.Hand.GetPile(card.Owner).Cards
            .Where(c => c.VisualCardPool.IsColorless).ToList();
        foreach (CardModel colorless in colorlessCards)
        {
            await CardCmd.Exhaust(choiceContext, colorless);
            await DamageCmd.Attack(card.DynamicVars.Damage.BaseValue).FromCard(card, cardPlay)
                .TargetingAllOpponents(card.CombatState!).Execute(choiceContext);
        }
    }
}

[HarmonyPatch(typeof(Conqueror), "OnPlay")]
public static class ConquerorPlayPatch
{
    public static bool Prefix(Conqueror __instance, PlayerChoiceContext choiceContext,
        ref Task __result)
    {
        __result = Play(__instance, choiceContext);
        return false;
    }

    private static async Task Play(Conqueror card, PlayerChoiceContext choiceContext)
    {
        await CreatureCmd.TriggerAnim(card.Owner.Creature, "Cast", card.Owner.Character.CastAnimDelay);
        await ForgeCmd.Forge(card.DynamicVars.Forge.IntValue, card.Owner, card);
        await PowerCmd.Apply<ConquerorPower>(choiceContext, card.Owner.Creature, 1m,
            card.Owner.Creature, card);
    }
}

[HarmonyPatch(typeof(ConquerorPower), "Type", MethodType.Getter)]
public static class ConquerorPowerTypePatch
{
    public static PowerType Postfix(PowerType __result) => PowerType.Buff;
}

[HarmonyPatch(typeof(ConquerorPower), "AfterSideTurnEnd")]
public static class ConquerorTurnEndPatch
{
    public static bool Prefix(ConquerorPower __instance, IEnumerable<Creature> participants,
        ref Task __result)
    {
        // Every stack granted this turn expires together, even after repeated plays.
        __result = participants.Contains(__instance.Owner)
            ? PowerCmd.Remove(__instance)
            : Task.CompletedTask;
        return false;
    }
}

[HarmonyPatch(typeof(CardModel), "GetEnchantedReplayCount")]
public static class ConquerorReplayPatch
{
    public static int Postfix(int __result, CardModel __instance)
    {
        // Forge hover tips use the canonical blade, which has no player owner.
        // Owner is only valid on mutable instances, including combat/upgrade previews.
        if (__instance is not SovereignBlade || !__instance.IsMutable || __instance.Owner?.Creature == null)
        {
            return __result;
        }
        return __result + (__instance.Owner.Creature.GetPower<ConquerorPower>()?.Amount ?? 0);
    }
}

[HarmonyPatch(typeof(Furnace), "OnUpgrade")]
public static class FurnaceUpgradePatch
{
    // The upgraded Power still forges five each turn.
    public static bool Prefix() => false;
}

[HarmonyPatch(typeof(CardPileCmd), "Draw", new[]
{
    typeof(PlayerChoiceContext), typeof(decimal), typeof(Player), typeof(bool)
})]
public static class FurnaceOpeningDrawOrderPatch
{
    public static void Prefix(Player player, bool fromHandDraw)
    {
        if (!fromHandDraw || player.PlayerCombatState is not { TurnNumber: 1 })
        {
            return;
        }

        // Imbued stays below opening-hand cards before its auto-pre-play hook runs.
        var drawPile = PileType.Draw.GetPile(player);
        foreach (CardModel card in drawPile.Cards.Where(c => c is Furnace && c.IsUpgraded).ToList())
        {
            drawPile.MoveToBottomInternal(card);
        }
    }
}

[HarmonyPatch(typeof(AbstractModel), "AfterAutoPrePlayPhaseEntered")]
public static class FurnaceOpeningAutoPlayPatch
{
    public static bool Prefix(AbstractModel __instance, PlayerChoiceContext choiceContext,
        MegaCrit.Sts2.Core.Entities.Players.Player player, ref Task __result)
    {
        if (__instance is not Furnace furnace || !furnace.IsUpgraded
            || furnace.Owner != player || player.PlayerCombatState is not { TurnNumber: <= 1 })
        {
            return true;
        }
        __result = CardCmd.AutoPlay(choiceContext, furnace, null);
        return false;
    }
}

[HarmonyPatch(typeof(Comet), "CanonicalVars", MethodType.Getter)]
public static class CometVarsPatch
{
    public static IEnumerable<DynamicVar> Postfix(IEnumerable<DynamicVar> __result)
        => new DynamicVar[]
        {
            new DamageVar(18m, MegaCrit.Sts2.Core.ValueProps.ValueProp.Move),
            new StarsVar(5)
        };
}

[HarmonyPatch(typeof(Comet), "OnUpgrade")]
public static class CometUpgradePatch
{
    public static bool Prefix(Comet __instance)
    {
        __instance.DynamicVars.Damage.UpgradeValueBy(5m);
        return false;
    }
}

[HarmonyPatch(typeof(Comet), "OnPlay")]
public static class CometPlayPatch
{
    public static bool Prefix(Comet __instance, PlayerChoiceContext choiceContext,
        CardPlay cardPlay, ref Task __result)
    {
        __result = Play(__instance, choiceContext, cardPlay);
        return false;
    }

    private static async Task Play(Comet card, PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(card.DynamicVars.Damage.BaseValue).FromCard(card, cardPlay)
            .Targeting(cardPlay.Target!).Execute(choiceContext);
        await PlayerCmd.GainStars(card.DynamicVars.Stars.BaseValue, card.Owner);
    }
}

[HarmonyPatch(typeof(ForegoneConclusion), "OnUpgrade")]
public static class ForegoneConclusionUpgradePatch
{
    public static bool Prefix(ForegoneConclusion __instance)
    {
        __instance.EnergyCost.UpgradeBy(-1);
        return false;
    }
}

[HarmonyPatch(typeof(ForegoneConclusion), "OnPlay")]
public static class ForegoneConclusionPlayPatch
{
    public static bool Prefix(ForegoneConclusion __instance, PlayerChoiceContext choiceContext,
        ref Task __result)
    {
        __result = Play(__instance, choiceContext);
        return false;
    }

    private static async Task Play(ForegoneConclusion card, PlayerChoiceContext choiceContext)
    {
        await CreatureCmd.TriggerAnim(card.Owner.Creature, "PowerUp", card.Owner.Character.PowerUpAnimDelay);
        VigorPower? vigor = card.Owner.Creature.GetPower<VigorPower>();
        if (vigor == null)
        {
            return;
        }
        int amount = vigor.Amount;
        if (amount > 0)
        {
            await PowerCmd.Apply<StrengthPower>(choiceContext, card.Owner.Creature, amount,
                card.Owner.Creature, card);
        }
        await PowerCmd.Remove(vigor);
    }
}
