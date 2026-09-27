using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace SilentRebalance.SilentRebalanceCode.Hooks;

[HarmonyPatch(typeof(CalculatedGamble), "OnUpgrade")]
public static class CalculatedGambleUpgradePatch
{
    public static void Postfix(CalculatedGamble __instance)
    {
        __instance.RemoveKeyword(CardKeyword.Exhaust);
        __instance.RemoveKeyword(CardKeyword.Retain);
    }
}

[HarmonyPatch(typeof(CorrosiveWavePower), "AfterCardDrawn")]
public static class CorrosiveWavePowerPatch
{
    public static bool Prefix(
        CorrosiveWavePower __instance,
        PlayerChoiceContext choiceContext,
        CardModel card,
        ref Task __result)
    {
        __result = card.Owner.Creature == __instance.Owner
            ? ApplyToRandomEnemy(__instance, choiceContext)
            : Task.CompletedTask;
        return false;
    }

    private static async Task ApplyToRandomEnemy(
        CorrosiveWavePower power,
        PlayerChoiceContext choiceContext)
    {
        Creature? target = power.Owner.Player!.RunState.Rng.CombatTargets
            .NextItem(power.CombatState.HittableEnemies);
        if (target != null)
        {
            await PowerCmd.Apply<PoisonPower>(
                choiceContext, target, power.Amount, power.Owner, null);
        }
    }
}

[HarmonyPatch(typeof(Untouchable), "CanonicalVars", MethodType.Getter)]
public static class UntouchableVarsPatch
{
    public static IEnumerable<DynamicVar> Postfix(IEnumerable<DynamicVar> __result)
        => __result.Select(v => v is BlockVar block ? new BlockVar(7m, block.Props) : v);
}

[HarmonyPatch(typeof(CardModel), "Rarity", MethodType.Getter)]
public static class ExpertiseRarityPatch
{
    public static CardRarity Postfix(CardRarity __result, CardModel __instance)
        => __instance is Expertise ? CardRarity.Uncommon : __result;
}

[HarmonyPatch(typeof(CardModel), "CanonicalEnergyCost", MethodType.Getter)]
public static class MasterPlannerCostPatch
{
    public static int Postfix(int __result, CardModel __instance)
        => __instance is MasterPlanner ? 1 : __result;
}

[HarmonyPatch(typeof(MasterPlannerPower), "AfterCardPlayed")]
public static class MasterPlannerPowerPatch
{
    private sealed class State
    {
        public int LastTriggeredTurn = -1;
    }

    private static readonly ConditionalWeakTable<MasterPlannerPower, State> States = new();

    public static bool Prefix(
        MasterPlannerPower __instance,
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay,
        ref Task __result)
    {
        __result = Task.CompletedTask;

        if (cardPlay.Card.Owner != __instance.Owner.Player
            || cardPlay.Card.Type != CardType.Skill
            || cardPlay.Card.IsSlyThisTurn)
        {
            return false;
        }

        State state = States.GetOrCreateValue(__instance);
        int turnNumber = __instance.Owner.Player!.PlayerCombatState!.TurnNumber;
        if (state.LastTriggeredTurn == turnNumber)
        {
            return false;
        }

        state.LastTriggeredTurn = turnNumber;
        CardCmd.ApplyKeyword(cardPlay.Card, CardKeyword.Sly);
        return false;
    }
}

[HarmonyPatch(typeof(CardModel), "CanonicalKeywords", MethodType.Getter)]
public static class WraithFormKeywordsPatch
{
    public static IEnumerable<CardKeyword> Postfix(
        IEnumerable<CardKeyword> __result,
        CardModel __instance)
    {
        if (__instance is not WraithForm)
        {
            return __result;
        }

        return (__result ?? Enumerable.Empty<CardKeyword>())
            .Concat(new[] { CardKeyword.Innate, CardKeyword.Retain, CardKeyword.Sly });
    }
}
