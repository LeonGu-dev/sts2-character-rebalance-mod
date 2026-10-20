using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.ValueProps;
using RebalanceMod.Necrobinder.Hooks;

namespace RebalanceMod.Necrobinder.Powers;

public sealed class OstyDeathDrawPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string CustomPackedIconPath => "res://RebalanceMod/Necrobinder/images/powers/death_draw.svg";
    public override string CustomBigIconPath => CustomPackedIconPath;

    public override Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature,
        bool wasRemovalPrevented, float deathAnimLength)
        => !wasRemovalPrevented && creature.Monster is Osty && creature.PetOwner == Owner.Player
            ? NecrobinderCombat.DrawOnDeath(choiceContext, Owner.Player!, Amount) : Task.CompletedTask;
}

public sealed class OstyDeathBlastPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string CustomPackedIconPath => "res://RebalanceMod/Necrobinder/images/powers/death_blast.svg";
    public override string CustomBigIconPath => CustomPackedIconPath;

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature,
        bool wasRemovalPrevented, float deathAnimLength)
    {
        if (!wasRemovalPrevented && creature.Monster is Osty && creature.PetOwner == Owner.Player
            && Owner.IsAlive && Owner.CombatState is { } combat)
            await CreatureCmd.Damage(choiceContext, combat.HittableEnemies, Amount, ValueProp.Unpowered, Owner);
    }
}

public sealed class PriceOfPowerPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override string CustomPackedIconPath => "res://RebalanceMod/Necrobinder/images/powers/price_of_power.svg";
    public override string CustomBigIconPath => CustomPackedIconPath;

    public override decimal ModifySummonAmount(Player summoner, decimal amount, AbstractModel? source)
        => summoner == Owner.Player ? 0m : amount;
}
