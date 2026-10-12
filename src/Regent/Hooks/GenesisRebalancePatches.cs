using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace RebalanceMod.Regent.Hooks;

internal static class GenesisState
{
    private sealed class Flags
    {
        public bool Active;
        public bool Locked;
    }

    private static readonly ConditionalWeakTable<GenesisPower, Flags> States = new();

    public static void Activate(GenesisPower power)
    {
        Flags state = States.GetOrCreateValue(power);
        if (!state.Locked)
        {
            state.Active = true;
        }
    }

    public static bool IsActive(GenesisPower? power)
        => power != null && States.TryGetValue(power, out Flags? state) && state.Active;

    public static bool IsLocked(GenesisPower? power)
        => power != null && States.TryGetValue(power, out Flags? state) && state.Locked;

    public static void Lock(GenesisPower power)
    {
        Flags state = States.GetOrCreateValue(power);
        state.Active = false;
        state.Locked = true;
    }

    public static bool IsActive(CardModel card)
        => card.Owner?.Creature != null && IsActive(card.Owner.Creature.GetPower<GenesisPower>());
}

[HarmonyPatch(typeof(Genesis), "CanonicalVars", MethodType.Getter)]
public static class GenesisVarsPatch
{
    public static IEnumerable<DynamicVar> Postfix(IEnumerable<DynamicVar> __result)
        => System.Array.Empty<DynamicVar>();
}

[HarmonyPatch(typeof(Genesis), "OnUpgrade")]
public static class GenesisUpgradePatch
{
    public static bool Prefix(Genesis __instance)
    {
        __instance.EnergyCost.UpgradeBy(-1);
        return false;
    }
}

[HarmonyPatch(typeof(Genesis), "OnPlay")]
public static class GenesisPlayPatch
{
    public static bool Prefix(Genesis __instance, PlayerChoiceContext choiceContext, ref Task __result)
    {
        __result = Play(__instance, choiceContext);
        return false;
    }

    private static async Task Play(Genesis card, PlayerChoiceContext choiceContext)
    {
        await CreatureCmd.TriggerAnim(card.Owner.Creature, "PowerUp", card.Owner.Character.PowerUpAnimDelay);
        GenesisPower? power = await PowerCmd.Apply<GenesisPower>(choiceContext,
            card.Owner.Creature, 1m, card.Owner.Creature, card);
        if (power != null)
        {
            GenesisState.Activate(power);
        }
    }
}

[HarmonyPatch(typeof(GenesisPower), "AfterEnergyReset")]
public static class GenesisOriginalGainPatch
{
    public static bool Prefix(ref Task __result)
    {
        __result = Task.CompletedTask;
        return false;
    }
}

[HarmonyPatch(typeof(AbstractModel), "AfterSideTurnEnd")]
public static class GenesisTurnEndPatch
{
    public static bool Prefix(AbstractModel __instance, CombatSide side,
        IEnumerable<Creature> participants, ref Task __result)
    {
        if (__instance is not GenesisPower power || !GenesisState.IsActive(power)
            || !System.Linq.Enumerable.Contains(participants, power.Owner))
        {
            return true;
        }
        __result = FinishTurn(power);
        return false;
    }

    private static async Task FinishTurn(GenesisPower power)
    {
        GenesisState.Lock(power);
        Player? player = power.Owner.Player;
        if (player?.PlayerCombatState != null)
        {
            await PlayerCmd.LoseStars(player.PlayerCombatState.Stars, player);
        }
    }
}

[HarmonyPatch(typeof(AbstractModel), "ShouldGainStars")]
public static class GenesisNoMoreStarsPatch
{
    public static bool Postfix(bool __result, AbstractModel __instance, Player player)
        => __result && (__instance is not GenesisPower power
            || power.Owner.Player != player || !GenesisState.IsLocked(power));
}

[HarmonyPatch(typeof(CardModel), "SpendStars")]
public static class GenesisNoSpendPatch
{
    public static bool Prefix(CardModel __instance, int amount, ref Task __result)
    {
        if (!GenesisState.IsActive(__instance))
        {
            return true;
        }
        // Preserve the effective X value while bypassing both loss and AfterStarsSpent hooks.
        __instance.LastStarsSpent = amount;
        __result = Task.CompletedTask;
        return false;
    }
}

[HarmonyPatch(typeof(CardModel), "SpendResources")]
public static class GenesisResourceRecordPatch
{
    public static void Postfix(CardModel __instance, ref Task<(int, int)> __result)
    {
        if (GenesisState.IsActive(__instance))
        {
            __result = MarkStarsUnspent(__result);
        }
    }

    private static async Task<(int, int)> MarkStarsUnspent(Task<(int, int)> original)
    {
        (int energy, _) = await original;
        return (energy, 0);
    }
}
