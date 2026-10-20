using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using RebalanceMod.Necrobinder.Powers;

namespace RebalanceMod.Necrobinder.Hooks;

internal static class NecrobinderCards
{
    public static readonly System.Type[] Changed =
    {
        typeof(Graveblast), typeof(Reave), typeof(Defy), typeof(PullAggro), typeof(RightHandHand),
        typeof(Fetch), typeof(Debilitate), typeof(PullFromBelow), typeof(Severance), typeof(Melancholy),
        typeof(DeathsDoor), typeof(DevourLife), typeof(SpiritOfAsh), typeof(BansheesCry), typeof(Misery),
        typeof(CaptureSpirit), typeof(Wisp), typeof(Reanimate), typeof(SentryMode)
    };

    public static IEnumerable<MethodBase> Declared(string name)
        => Changed.Select(type => AccessTools.DeclaredMethod(type, name)).OfType<MethodBase>();

    public static bool IsReworkedPower(CardModel card) => card is PullFromBelow or DeathsDoor or Misery;

    public static decimal OstyHp(CardModel card)
        => card.IsMutable && card.Owner?.IsOstyAlive == true ? card.Owner.Osty!.CurrentHp : 0;

    public static decimal Healing(CardModel card)
        => System.Math.Min(NecrobinderCombat.Deaths(card) * card.DynamicVars["HealPerDeath"].BaseValue,
            card.DynamicVars["HealCap"].BaseValue);
}

[HarmonyPatch]
public static class NecrobinderVarsPatch
{
    public static IEnumerable<MethodBase> TargetMethods() => NecrobinderCards.Declared("get_CanonicalVars");

    public static IEnumerable<DynamicVar> Postfix(IEnumerable<DynamicVar> __result, CardModel __instance)
        => __instance switch
        {
            Graveblast => new DynamicVar[] { new DamageVar(8m, ValueProp.Move) },
            Reave => new DynamicVar[] { new DamageVar(6m, ValueProp.Move), new CardsVar(1) },
            Defy => new DynamicVar[] { new BlockVar(8m, ValueProp.Move), new PowerVar<WeakPower>(2m) },
            PullAggro => new DynamicVar[] { new SummonVar(7m), new BlockVar(4m, ValueProp.Move) },
            RightHandHand => new DynamicVar[] { new OstyDamageVar(6m, ValueProp.Move), new EnergyVar(2) },
            Fetch => new DynamicVar[] { new OstyDamageVar(4m, ValueProp.Move), new SummonVar(4m) },
            Debilitate => new DynamicVar[] { new DamageVar(6m, ValueProp.Move), new PowerVar<DebilitatePower>(3m) },
            PullFromBelow => new DynamicVar[] { new PowerVar<OstyDeathDrawPower>(1m) },
            Severance => new DynamicVar[] { new BlockVar(2m, ValueProp.Move), new PowerVar<DoomPower>(3m) },
            Melancholy => new DynamicVar[]
            {
                new CalculationBaseVar(0m), new CalculationExtraVar(1m),
                new CalculatedVar("OstyHp").WithMultiplier((card, _) => NecrobinderCards.OstyHp(card))
            },
            DeathsDoor => new DynamicVar[]
            {
                new DynamicVar("HealPerDeath", 1m), new DynamicVar("HealCap", 6m),
                new CalculationBaseVar(0m), new CalculationExtraVar(1m),
                new CalculatedVar("CurrentHeal").WithMultiplier((card, _) => NecrobinderCards.Healing(card))
            },
            SpiritOfAsh => new DynamicVar[] { new DynamicVar("BlockOnExhaust", 3m) },
            BansheesCry => new DynamicVar[]
            {
                new CalculationBaseVar(16m), new ExtraDamageVar(1m),
                new CalculatedDamageVar(ValueProp.Move).WithMultiplier((card, _) => NecrobinderCards.OstyHp(card))
            },
            Misery => new DynamicVar[] { new PowerVar<OstyDeathBlastPower>(7m) },
            CaptureSpirit => new DynamicVar[]
            {
                new DamageVar(3m, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move),
                new SummonVar(2m), new CardsVar(1)
            },
            Wisp => new DynamicVar[] { new SummonVar(4m) },
            _ => __result
        };
}

[HarmonyPatch]
public static class NecrobinderUpgradePatch
{
    public static IEnumerable<MethodBase> TargetMethods() => NecrobinderCards.Declared("OnUpgrade");

    public static bool Prefix(CardModel __instance)
    {
        switch (__instance)
        {
            case Reave:
                __instance.DynamicVars.Damage.UpgradeValueBy(2m);
                break;
            case Defy:
                __instance.DynamicVars.Block.UpgradeValueBy(3m);
                break;
            case PullAggro:
                __instance.DynamicVars.Summon.UpgradeValueBy(2m);
                __instance.DynamicVars.Block.UpgradeValueBy(1m);
                break;
            case Fetch:
                __instance.DynamicVars.OstyDamage.UpgradeValueBy(2m);
                __instance.DynamicVars.Summon.UpgradeValueBy(2m);
                break;
            case Debilitate:
                __instance.DynamicVars.Damage.UpgradeValueBy(3m);
                __instance.DynamicVars["DebilitatePower"].UpgradeValueBy(1m);
                break;
            case PullFromBelow:
                __instance.DynamicVars["OstyDeathDrawPower"].UpgradeValueBy(1m);
                break;
            case Severance:
                __instance.DynamicVars.Block.UpgradeValueBy(1m);
                __instance.DynamicVars.Doom.UpgradeValueBy(-1m);
                break;
            case Melancholy:
                __instance.EnergyCost.UpgradeBy(-1);
                break;
            case DeathsDoor:
                __instance.DynamicVars["HealPerDeath"].UpgradeValueBy(1m);
                __instance.DynamicVars["HealCap"].UpgradeValueBy(3m);
                break;
            case BansheesCry:
                __instance.DynamicVars.CalculationBase.UpgradeValueBy(4m);
                break;
            case Misery:
                __instance.DynamicVars["OstyDeathBlastPower"].UpgradeValueBy(4m);
                break;
            case CaptureSpirit:
                __instance.DynamicVars.Damage.UpgradeValueBy(1m);
                __instance.DynamicVars.Summon.UpgradeValueBy(1m);
                break;
            case Wisp:
                __instance.DynamicVars.Summon.UpgradeValueBy(2m);
                __instance.RemoveKeyword(CardKeyword.Retain);
                __instance.RemoveKeyword(CardKeyword.Exhaust);
                break;
            default:
                return true;
        }
        return false;
    }
}

[HarmonyPatch(typeof(CardModel), "Type", MethodType.Getter)]
public static class NecrobinderTypePatch
{
    public static CardType Postfix(CardType __result, CardModel __instance)
        => NecrobinderCards.IsReworkedPower(__instance) ? CardType.Power
            : __instance is Severance ? CardType.Skill : __result;
}

[HarmonyPatch(typeof(CardModel), "Rarity", MethodType.Getter)]
public static class NecrobinderRarityPatch
{
    public static CardRarity Postfix(CardRarity __result, CardModel __instance)
        => __instance is Defy or Misery ? CardRarity.Uncommon : __result;
}

[HarmonyPatch(typeof(CardModel), "CanonicalEnergyCost", MethodType.Getter)]
public static class NecrobinderCostPatch
{
    public static int Postfix(int __result, CardModel __instance)
        => __instance switch
        {
            Severance or Misery => 1,
            Melancholy or BansheesCry => 2,
            _ => __result
        };
}

[HarmonyPatch(typeof(CardModel), "TargetType", MethodType.Getter)]
public static class NecrobinderTargetPatch
{
    public static TargetType Postfix(TargetType __result, CardModel __instance)
        => __instance switch
        {
            PullFromBelow or Severance or Melancholy or DeathsDoor or Misery => TargetType.Self,
            CaptureSpirit => TargetType.AllEnemies,
            Fetch when __instance.IsMutable && __instance.Owner?.IsOstyMissing == true => TargetType.Self,
            _ => __result
        };
}

[HarmonyPatch(typeof(Wisp), "CanonicalKeywords", MethodType.Getter)]
public static class WispKeywordsPatch
{
    public static IEnumerable<CardKeyword> Postfix(IEnumerable<CardKeyword> __result)
        => __result.Where(keyword => keyword is not CardKeyword.Exhaust and not CardKeyword.Retain);
}

[HarmonyPatch(typeof(Fetch), "ShouldGlowRedInternal", MethodType.Getter)]
public static class BoneDanceGlowPatch
{
    public static bool Prefix(ref bool __result)
    {
        __result = false;
        return false;
    }
}

[HarmonyPatch]
public static class NecrobinderPlayPatch
{
    private static readonly System.Type[] Reworks =
    {
        typeof(Reave), typeof(Fetch), typeof(PullFromBelow), typeof(Severance), typeof(Melancholy),
        typeof(DeathsDoor), typeof(BansheesCry), typeof(Misery), typeof(CaptureSpirit), typeof(Wisp), typeof(Reanimate)
    };

    public static IEnumerable<MethodBase> TargetMethods()
        => Reworks.Select(type => AccessTools.DeclaredMethod(type, "OnPlay"));

    public static bool Prefix(CardModel __instance, PlayerChoiceContext choiceContext,
        CardPlay cardPlay, ref Task __result)
    {
        __result = Play(__instance, choiceContext, cardPlay);
        return false;
    }

    private static async Task Play(CardModel card, PlayerChoiceContext context, CardPlay play)
    {
        switch (card)
        {
            case Reave:
                if (play.Target != null)
                    await DamageCmd.Attack(card.DynamicVars.Damage.BaseValue).FromCard(card, play)
                        .Targeting(play.Target).WithHitFx("vfx/vfx_attack_slash").Execute(context);
                var soul = card.CombatState!.CreateCard<Soul>(card.Owner);
                if (card.IsUpgraded) CardCmd.Upgrade(soul);
                await CardPileCmd.AddGeneratedCardToCombat(soul, PileType.Hand, card.Owner);
                break;
            case Fetch:
                if (card.Owner.IsOstyMissing)
                    await OstyCmd.Summon(context, card.Owner, card.DynamicVars.Summon.BaseValue, card);
                else if (play.Target != null)
                    await DamageCmd.Attack(card.DynamicVars.OstyDamage.BaseValue)
                        .FromOsty(card.Owner.Osty!, card, play).Targeting(play.Target)
                        .WithHitFx("vfx/vfx_attack_blunt").Execute(context);
                break;
            case PullFromBelow:
                await PowerCmd.Apply<OstyDeathDrawPower>(context, card.Owner.Creature,
                    card.DynamicVars["OstyDeathDrawPower"].BaseValue, card.Owner.Creature, card);
                break;
            case Severance:
                await KillOsty(card);
                if (CombatManager.Instance.IsOverOrEnding) break;
                var hand = PileType.Hand.GetPile(card.Owner);
                var originals = hand.Cards.Where(original => original.Type != CardType.Attack
                    && original.IsTransformable).ToList();
                foreach (CardModel original in originals)
                {
                    if (original.Pile != hand) continue;
                    var replacement = card.CombatState!.CreateCard<Soul>(card.Owner);
                    if (card.IsUpgraded) CardCmd.Upgrade(replacement);
                    var result = await CardCmd.Transform(original, replacement);
                    if (result?.success != true) continue;
                    await CreatureCmd.GainBlock(card.Owner.Creature, card.DynamicVars.Block, play);
                    await PowerCmd.Apply<DoomPower>(context, card.Owner.Creature,
                        card.DynamicVars.Doom.BaseValue, card.Owner.Creature, card);
                }
                break;
            case Melancholy:
                decimal hp = NecrobinderCards.OstyHp(card);
                if (hp <= 0) break;
                foreach (Creature enemy in card.CombatState!.HittableEnemies.ToList())
                    await PowerCmd.Apply<DoomPower>(context, enemy, hp, card.Owner.Creature, card);
                await KillOsty(card);
                break;
            case DeathsDoor:
                await CreatureCmd.Heal(card.Owner.Creature, NecrobinderCards.Healing(card));
                break;
            case BansheesCry:
                await DamageCmd.Attack(card.DynamicVars.CalculatedDamage).FromCard(card, play)
                    .TargetingAllOpponents(card.CombatState!).WithHitFx("vfx/vfx_attack_slash").Execute(context);
                await KillOsty(card);
                await PowerCmd.Apply<PriceOfPowerPower>(context, card.Owner.Creature, 1m, card.Owner.Creature, card);
                break;
            case Misery:
                await PowerCmd.Apply<OstyDeathBlastPower>(context, card.Owner.Creature,
                    card.DynamicVars["OstyDeathBlastPower"].BaseValue, card.Owner.Creature, card);
                break;
            case CaptureSpirit:
                var enemies = card.CombatState!.Enemies.Where(enemy => enemy.IsAlive).ToList();
                await CreatureCmd.Damage(context, card.CombatState.HittableEnemies.ToList(),
                    card.DynamicVars.Damage, card.Owner.Creature);
                for (int index = 0; index < enemies.Count; index++)
                {
                    if (CombatManager.Instance.IsOverOrEnding) break;
                    await OstyCmd.Summon(context, card.Owner, card.DynamicVars.Summon.BaseValue, card);
                    await Soul.CreateInHand(card.Owner, 1, card.CombatState);
                }
                break;
            case Wisp:
                await OstyCmd.Summon(context, card.Owner, card.DynamicVars.Summon.BaseValue, card);
                break;
            case Reanimate:
                int repeats = card.Owner.IsOstyMissing ? 2 : 1;
                for (int index = 0; index < repeats; index++)
                    await OstyCmd.Summon(context, card.Owner, card.DynamicVars.Summon.BaseValue, card);
                break;
        }
    }

    private static async Task KillOsty(CardModel card)
    {
        if (card.Owner.IsOstyAlive) await CreatureCmd.Kill(card.Owner.Osty!);
    }
}

[HarmonyPatch]
public static class NecrobinderRemovedHooksPatch
{
    public static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.DeclaredMethod(typeof(Melancholy), nameof(AbstractModel.AfterDeath));
        yield return AccessTools.DeclaredMethod(typeof(BansheesCry), nameof(AbstractModel.AfterCardEnteredCombat));
        yield return AccessTools.DeclaredMethod(typeof(BansheesCry), nameof(AbstractModel.AfterCardPlayed));
        yield return AccessTools.DeclaredMethod(typeof(SentryModePower), nameof(AbstractModel.BeforeHandDraw));
    }

    public static bool Prefix(ref Task __result)
    {
        __result = Task.CompletedTask;
        return false;
    }
}

[HarmonyPatch]
public static class NecrobinderHoverTipsPatch
{
    public static IEnumerable<MethodBase> TargetMethods()
        => new[] { typeof(PullFromBelow), typeof(Severance), typeof(Melancholy), typeof(DevourLifePower), typeof(Wisp) }
            .Select(type => AccessTools.DeclaredMethod(type, "get_ExtraHoverTips")).OfType<MethodBase>();

    public static IEnumerable<IHoverTip> Postfix(IEnumerable<IHoverTip> __result, AbstractModel __instance)
        => __instance switch
        {
            PullFromBelow => new[] { HoverTipFactory.FromPower<OstyDeathDrawPower>() },
            Severance card => new[] { HoverTipFactory.FromCard<Soul>(card.IsUpgraded), HoverTipFactory.FromPower<DoomPower>() },
            Melancholy or DevourLifePower => new[] { HoverTipFactory.FromPower<DoomPower>() },
            Wisp => new[] { HoverTipFactory.Static(StaticHoverTip.SummonStatic) },
            _ => __result
        };
}

[HarmonyPatch]
public static class NecrobinderBlockTagPatch
{
    public static IEnumerable<MethodBase> TargetMethods()
        => new[] { typeof(CardModel), typeof(Melancholy), typeof(DeathsDoor), typeof(Severance) }
            .Select(type => AccessTools.DeclaredMethod(type, "get_GainsBlock")).OfType<MethodBase>();

    public static bool Prefix(CardModel __instance, ref bool __result)
    {
        if (__instance is not (Melancholy or DeathsDoor or Severance)) return true;
        __result = __instance is Severance;
        return false;
    }
}

[HarmonyPatch(typeof(CardModel), "ExtraHoverTips", MethodType.Getter)]
public static class NecrobinderInheritedHoverTipsPatch
{
    public static IEnumerable<IHoverTip> Postfix(IEnumerable<IHoverTip> __result, CardModel __instance)
        => __instance switch
        {
            Misery => new[] { HoverTipFactory.FromPower<OstyDeathBlastPower>() },
            BansheesCry => new[] { HoverTipFactory.FromPower<PriceOfPowerPower>() },
            _ => __result
        };
}
