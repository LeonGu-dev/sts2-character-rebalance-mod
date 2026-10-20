using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;

namespace RebalanceMod.Necrobinder.Hooks;

internal static class NecrobinderCombat
{
    private sealed class PlayerData
    {
        public int Deaths;
        public bool BeforeFlush;
        public decimal PendingDraw;
    }

    private sealed class CombatData
    {
        public readonly Dictionary<Player, PlayerData> Players = new();
    }

    private static readonly ConditionalWeakTable<ICombatState, CombatData> States = new();

    private static PlayerData Data(ICombatState combat, Player player)
    {
        var players = States.GetOrCreateValue(combat).Players;
        if (!players.TryGetValue(player, out PlayerData? data))
            players[player] = data = new PlayerData();
        return data;
    }

    public static int Deaths(CardModel card)
        => card.IsMutable && card.CombatState is { } combat && card.Owner is { } player
            ? Data(combat, player).Deaths : 0;

    public static void RecordDeath(ICombatState? combat, Creature creature, bool prevented)
    {
        if (!prevented && combat != null && creature.Monster is Osty && creature.PetOwner is { } player)
            Data(combat, player).Deaths++;
    }

    public static void BeginEndTurn(ICombatState combat, Player player) => Data(combat, player).BeforeFlush = true;

    public static void MarkFlushed(ICombatState combat, Player player) => Data(combat, player).BeforeFlush = false;

    public static Task DrawOnDeath(PlayerChoiceContext context, Player player, decimal amount)
    {
        if (player.Creature.IsDead || player.Creature.CombatState is not { } combat
            || CombatManager.Instance.IsOverOrEnding)
            return Task.CompletedTask;

        var data = Data(combat, player);
        if (combat.CurrentSide == CombatSide.Player && data.BeforeFlush)
        {
            data.PendingDraw += amount;
            return Task.CompletedTask;
        }
        return CardPileCmd.Draw(context, amount, player);
    }

    public static Task FlushDraws(ICombatState combat, Player player, PlayerChoiceContext context)
    {
        var data = Data(combat, player);
        decimal amount = data.PendingDraw;
        data.PendingDraw = 0m;
        return amount > 0 && player.Creature.IsAlive && !CombatManager.Instance.IsOverOrEnding
            ? CardPileCmd.Draw(context, amount, player) : Task.CompletedTask;
    }

    public static void Clear(ICombatState? combat)
    {
        if (combat != null) States.Remove(combat);
    }
}

[HarmonyPatch(typeof(Hook), nameof(Hook.AfterDeath))]
public static class OstyDeathCountPatch
{
    public static void Prefix(ICombatState? combatState, Creature creature, bool wasRemovalPrevented)
        => NecrobinderCombat.RecordDeath(combatState, creature, wasRemovalPrevented);
}

[HarmonyPatch(typeof(Hook), nameof(Hook.AfterAutoPostPlayPhaseEntered))]
public static class OstyEndTurnDrawStartPatch
{
    public static void Prefix(ICombatState combatState, Player player)
        => NecrobinderCombat.BeginEndTurn(combatState, player);
}

[HarmonyPatch(typeof(Hook), nameof(Hook.AfterFlush))]
public static class OstyEndTurnDrawFlushPatch
{
    public static void Prefix(ICombatState combatState, Player player)
        => NecrobinderCombat.MarkFlushed(combatState, player);

    public static async Task Postfix(Task __result, ICombatState combatState, Player player,
        PlayerChoiceContext playerChoiceContext)
    {
        await __result;
        await NecrobinderCombat.FlushDraws(combatState, player, playerChoiceContext);
    }
}

[HarmonyPatch(typeof(Hook), nameof(Hook.AfterCombatEnd))]
public static class OstyCombatCleanupPatch
{
    public static async Task Postfix(Task __result, ICombatState? combatState)
    {
        await __result;
        NecrobinderCombat.Clear(combatState);
    }
}
