using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using DefectRebalance.DefectRebalanceCode.Extensions;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace DefectRebalance.DefectRebalanceCode.Powers;

/// <summary>
/// 折射(Refract)重做后的能力:当你将要受到伤害时,激发一颗计数不为0的玻璃充能球,并阻止此伤害。
/// 玻璃球计数 = 私有字段 _passiveVal;激发时玻璃球照常触发其Evoke(对全体造成2×计数伤害)。
/// </summary>
public class RefractReworkPower : DefectRebalancePower
{
    private static readonly FieldInfo GlassCounter = AccessTools.Field(typeof(GlassOrb), "_passiveVal");

    private bool _preventPending;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override List<(string, string)>? Localization => new PowerLoc(
        "折射",
        "当你将要受到伤害时，激发一颗计数不为0的[gold]玻璃[/gold]充能球，并阻止此伤害。",
        "当你将要受到伤害时，激发一颗计数不为0的[gold]玻璃[/gold]充能球，并阻止此伤害。");

    public override decimal ModifyHpLostAfterOstyLate(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target == base.Owner && HasGlassOrbAvailable())
        {
            _preventPending = true;
            return 0m;
        }
        return amount;
    }

    public override async Task AfterModifyingHpLostAfterOsty()
    {
        if (_preventPending)
        {
            _preventPending = false;
            await EvokeAGlassOrb();
        }
    }

    private bool HasGlassOrbAvailable()
        => base.Owner!.Player!.PlayerCombatState!.OrbQueue.Orbs.OfType<GlassOrb>().Any(orb => GetCounter(orb) > 0m);

    private static decimal GetCounter(GlassOrb orb)
        => (decimal)(GlassCounter.GetValue(orb) ?? 0m);

    private async Task EvokeAGlassOrb()
    {
        var queue = base.Owner!.Player!.PlayerCombatState!.OrbQueue;
        GlassOrb? orb = queue.Orbs.OfType<GlassOrb>().FirstOrDefault(o => GetCounter(o) > 0m);
        if (orb == null)
        {
            return;
        }
        bool removed = queue.Remove(orb);
        NCombatRoom.Instance?.GetCreatureNode(base.Owner)?.OrbManager?.EvokeOrbAnim(orb);
        var choiceContext = new BlockingPlayerChoiceContext();
        choiceContext.PushModel(orb);
        IEnumerable<Creature> targets = await orb.Evoke(choiceContext);
        choiceContext.PopModel(orb);
        if (base.Owner!.CombatState != null)
        {
            await Hook.AfterOrbEvoked(choiceContext, base.Owner.CombatState, orb, targets);
            if (removed)
            {
                orb.RemoveInternal();
            }
        }
    }
}
