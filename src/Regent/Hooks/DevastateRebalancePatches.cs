using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace RebalanceMod.Regent.Hooks;

internal static class DevastateGrowth
{
    private const string SavedKey = "IncreasedDamage";
    private const string StarIcon = "[img]res://images/packed/sprite_fonts/star_icon.png[/img]";

    private sealed class State
    {
        public int Bonus;
    }

    private static readonly ConditionalWeakTable<Devastate, State> States = new();

    public static int GetBonus(Devastate card)
        => States.TryGetValue(card, out State? state) ? state.Bonus : 0;

    public static int GetStars(Devastate card) => 1 + GetBonus(card);

    public static string GetDisplay(Devastate card)
    {
        int stars = GetStars(card);
        return stars <= 5 ? string.Concat(Enumerable.Repeat(StarIcon, stars)) : $"{stars}{StarIcon}";
    }

    public static void SetBonus(Devastate card, int bonus)
    {
        States.GetOrCreateValue(card).Bonus = bonus;
        card.DynamicVars.Stars.BaseValue = 1 + bonus;
        ((StringVar)card.DynamicVars["StarGain"]).StringValue = GetDisplay(card);
    }

    public static void Grow(Devastate card)
    {
        SetBonus(card, GetBonus(card) + 1);
        if (card.DeckVersion is Devastate deckCard)
        {
            SetBonus(deckCard, GetBonus(deckCard) + 1);
        }
    }

    public static void Copy(Devastate source, Devastate destination)
        => SetBonus(destination, GetBonus(source));

    // TheScythe already registers this integer property name for save/network data.
    // Write it at CardModel's serialization boundary, not the small From wrapper
    // which can be inlined by callers before Harmony patches it.
    public static void AddToSave(Devastate card, ref SavedProperties? props)
    {
        int bonus = GetBonus(card);
        if (bonus == 0)
        {
            return;
        }
        props ??= new SavedProperties();
        props.ints ??= new List<SavedProperties.SavedProperty<int>>();
        props.ints.RemoveAll(property => property.name == SavedKey);
        props.ints.Add(new SavedProperties.SavedProperty<int>(SavedKey, bonus));
    }

    public static void ReadFromSave(Devastate card, SavedProperties props)
    {
        if (props.ints == null)
        {
            return;
        }
        foreach (SavedProperties.SavedProperty<int> entry in props.ints)
        {
            if (entry.name == SavedKey)
            {
                SetBonus(card, entry.value);
                return;
            }
        }
    }
}

[HarmonyPatch(typeof(CardModel), "Rarity", MethodType.Getter)]
public static class DevastateRarityPatch
{
    public static CardRarity Postfix(CardRarity __result, CardModel __instance)
        => __instance is Devastate ? CardRarity.Rare : __result;
}

[HarmonyPatch(typeof(CardModel), "CanonicalEnergyCost", MethodType.Getter)]
public static class DevastateEnergyCostPatch
{
    public static int Postfix(int __result, CardModel __instance)
        => __instance is Devastate ? 2 : __result;
}

[HarmonyPatch(typeof(Devastate), "CanonicalStarCost", MethodType.Getter)]
public static class DevastateStarCostPatch
{
    public static int Postfix(int __result) => -1;
}

[HarmonyPatch(typeof(CardModel), "CanonicalKeywords", MethodType.Getter)]
public static class DevastateExhaustPatch
{
    public static IEnumerable<CardKeyword> Postfix(IEnumerable<CardKeyword> __result, CardModel __instance)
        => __instance is Devastate
            ? __result.Concat(new[] { CardKeyword.Exhaust })
            : __result;
}

[HarmonyPatch(typeof(Devastate), "CanonicalVars", MethodType.Getter)]
public static class DevastateVarsPatch
{
    public static IEnumerable<DynamicVar> Postfix(IEnumerable<DynamicVar> __result, Devastate __instance)
        => new DynamicVar[]
        {
            new DamageVar(10m, ValueProp.Move),
            new StarsVar(DevastateGrowth.GetStars(__instance)),
            new StringVar("StarGain", DevastateGrowth.GetDisplay(__instance))
        };
}

[HarmonyPatch(typeof(Devastate), "OnUpgrade")]
public static class DevastateUpgradePatch
{
    public static bool Prefix(Devastate __instance)
    {
        __instance.DynamicVars.Damage.UpgradeValueBy(4m);
        return false;
    }
}

[HarmonyPatch(typeof(Devastate), "OnPlay")]
public static class DevastatePlayPatch
{
    public static bool Prefix(Devastate __instance, PlayerChoiceContext choiceContext,
        CardPlay cardPlay, ref Task __result)
    {
        __result = Play(__instance, choiceContext, cardPlay);
        return false;
    }

    private static async Task Play(Devastate card, PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var target = cardPlay.Target;
        if (target == null)
        {
            return;
        }
        // Evaluate before damage, as Feed/TheHunt do: death can remove powers.
        bool shouldTriggerFatal = target.Powers.All(power => power.ShouldOwnerDeathTriggerFatal());
        int starsNow = DevastateGrowth.GetStars(card);
        var attack = await DamageCmd.Attack(card.DynamicVars.Damage.BaseValue).FromCard(card, cardPlay)
            .Targeting(target).WithHitFx("vfx/vfx_attack_blunt", null, "blunt_attack.mp3")
            .Execute(choiceContext);
        await PlayerCmd.GainStars(starsNow, card.Owner);
        if (shouldTriggerFatal && attack.Results.SelectMany(hit => hit)
            .Any(result => result.Receiver == target && result.WasTargetKilled))
        {
            DevastateGrowth.Grow(card);
        }
    }
}

[HarmonyPatch(typeof(AbstractModel), "MutableClone")]
public static class DevastateCloneGrowthPatch
{
    public static void Postfix(AbstractModel __instance, AbstractModel __result)
    {
        if (__instance is Devastate source && __result is Devastate destination)
        {
            DevastateGrowth.Copy(source, destination);
        }
    }
}

[HarmonyPatch(typeof(CardModel), "ToSerializable")]
public static class DevastateSaveGrowthPatch
{
    public static void Postfix(CardModel __instance, SerializableCard __result)
    {
        if (__instance is Devastate card)
        {
            SavedProperties? props = __result.Props;
            DevastateGrowth.AddToSave(card, ref props);
            __result.Props = props;
        }
    }
}

[HarmonyPatch(typeof(CardModel), "FromSerializable")]
public static class DevastateLoadGrowthPatch
{
    public static void Postfix(SerializableCard save, CardModel __result)
    {
        if (__result is Devastate card && save.Props != null)
        {
            DevastateGrowth.ReadFromSave(card, save.Props);
        }
    }
}
