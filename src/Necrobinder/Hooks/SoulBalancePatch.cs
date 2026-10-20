using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace RebalanceMod.Necrobinder.Hooks;

[HarmonyPatch(typeof(Soul), "CanonicalKeywords", MethodType.Getter)]
public static class SoulKeywordsPatch
{
    public static IEnumerable<CardKeyword> Postfix(IEnumerable<CardKeyword> __result)
        => __result.Append(CardKeyword.Ethereal).Distinct();
}

[HarmonyPatch(typeof(Soul), "OnPlay")]
public static class SoulPlayPatch
{
    public static bool Prefix(ref Task __result)
    {
        __result = Task.CompletedTask;
        return false;
    }
}

[HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.AfterCardExhausted))]
public static class SoulExhaustPatch
{
    public static bool Prefix(AbstractModel __instance, PlayerChoiceContext choiceContext,
        CardModel card, bool causedByEthereal, ref Task __result)
    {
        if (__instance is not Soul soul || !ReferenceEquals(soul, card))
        {
            return true;
        }

        if (causedByEthereal)
        {
            SoulDeferredDraw.Queue(soul);
            __result = Task.CompletedTask;
        }
        else
        {
            __result = CardPileCmd.Draw(choiceContext, soul.DynamicVars.Cards.BaseValue, soul.Owner);
        }
        return false;
    }
}

[HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.AfterSideTurnEnd))]
public static class SoulTurnEndDrawPatch
{
    public static bool Prefix(AbstractModel __instance, PlayerChoiceContext choiceContext,
        IEnumerable<Creature> participants, ref Task __result)
    {
        if (__instance is not Soul soul || !SoulDeferredDraw.HasPending(soul)
            || !participants.Contains(soul.Owner.Creature))
        {
            return true;
        }

        __result = SoulDeferredDraw.Draw(choiceContext, soul);
        return false;
    }
}

[HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.AfterCombatEnd))]
public static class SoulCombatEndPatch
{
    public static void Prefix(AbstractModel __instance)
    {
        if (__instance is Soul soul)
        {
            SoulDeferredDraw.Clear(soul);
        }
    }
}

internal static class SoulDeferredDraw
{
    private sealed class PendingDraw
    {
        public decimal Amount;
    }

    private static readonly ConditionalWeakTable<Soul, PendingDraw> Pending = new();

    public static void Queue(Soul soul)
        => Pending.GetOrCreateValue(soul).Amount += soul.DynamicVars.Cards.BaseValue;

    public static bool HasPending(Soul soul) => Pending.TryGetValue(soul, out _);

    public static Task Draw(PlayerChoiceContext choiceContext, Soul soul)
    {
        if (!Pending.TryGetValue(soul, out PendingDraw? pending))
        {
            return Task.CompletedTask;
        }

        Pending.Remove(soul);
        return CardPileCmd.Draw(choiceContext, pending.Amount, soul.Owner);
    }

    public static void Clear(Soul soul) => Pending.Remove(soul);
}
