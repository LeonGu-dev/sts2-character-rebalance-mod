using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using RebalanceMod.Necrobinder.Powers;

namespace RebalanceMod.Necrobinder.Hooks;

[HarmonyPatch(typeof(OstyCmd), nameof(OstyCmd.Summon))]
public static class PriceOfPowerSummonPatch
{
    public static bool Prefix(Player summoner, ref Task<SummonResult> __result)
    {
        if (!summoner.Creature.HasPower<PriceOfPowerPower>()) return true;
        __result = Task.FromResult(new SummonResult(summoner.Osty, 0m));
        return false;
    }
}

[HarmonyPatch(typeof(DevourLifePower), nameof(AbstractModel.AfterCardPlayed))]
public static class SoulCursePowerPatch
{
    public static bool Prefix(DevourLifePower __instance, PlayerChoiceContext choiceContext,
        CardPlay cardPlay, ref Task __result)
    {
        __result = Apply(__instance, choiceContext, cardPlay);
        return false;
    }

    private static async Task Apply(DevourLifePower power, PlayerChoiceContext context, CardPlay play)
    {
        if (play.Card is not Soul || play.Card.Owner.Creature != power.Owner) return;
        foreach (Creature enemy in power.Owner.CombatState!.HittableEnemies.ToArray())
            await PowerCmd.Apply<DoomPower>(context, enemy, power.Amount, power.Owner, play.Card);
    }
}

[HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.AfterCardExhausted))]
public static class SpiritOfAshExhaustPatch
{
    public static bool Prefix(AbstractModel __instance, CardModel card, ref Task __result)
    {
        if (__instance is not SpiritOfAshPower power) return true;
        __result = card.Owner == power.Owner.Player && card.Keywords.Contains(CardKeyword.Ethereal)
            ? CreatureCmd.GainBlock(power.Owner, power.Amount, ValueProp.Unpowered, null)
            : Task.CompletedTask;
        return false;
    }
}

[HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.AfterOstyRevived))]
public static class WakefulPowerPatch
{
    public static bool Prefix(AbstractModel __instance, Creature osty, ref Task __result)
    {
        if (__instance is not SentryModePower power) return true;
        __result = Generate(power, osty);
        return false;
    }

    private static async Task Generate(SentryModePower power, Creature osty)
    {
        if (osty.PetOwner is not { } player || player != power.Owner.Player) return;
        for (int index = 0; index < power.Amount; index++)
        {
            var card = osty.CombatState!.CreateCard<SweepingGaze>(player);
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, player);
        }
    }
}
