using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using BaseLib.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Orbs;

namespace DefectRebalance.DefectRebalanceCode.Cards;

/// <summary>
/// 递归(Recursion,STS1回归,新增卡)。
/// 1费(升级0)罕见技能:激发你[最右]的充能球,然后生成一个与被激发球完全相同的充能球。
/// </summary>
[Pool(typeof(DefectCardPool))]
public sealed class Recursion : DefectRebalanceCard
{
    private static readonly FieldInfo DarkEvokeField = AccessTools.Field(typeof(DarkOrb), "_evokeVal");
    private static readonly FieldInfo GlassCounterField = AccessTools.Field(typeof(GlassOrb), "_passiveVal");

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[] { HoverTipFactory.Static(StaticHoverTip.Evoke) };

    public Recursion() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var queue = base.Owner!.PlayerCombatState!.OrbQueue;
        OrbModel? orb = queue.Orbs.FirstOrDefault();
        if (orb == null)
        {
            return;
        }

        decimal darkEvoke = orb is DarkOrb d ? (decimal)DarkEvokeField.GetValue(d)! : 0m;
        decimal glassCounter = orb is GlassOrb g ? (decimal)GlassCounterField.GetValue(g)! : 0m;

        await OrbCmd.EvokeNext(choiceContext, base.Owner);

        OrbModel copy = CreateMutableCopy(orb.GetType(), darkEvoke, glassCounter);
        await OrbCmd.Channel(choiceContext, copy, base.Owner);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }

    private static OrbModel CreateMutableCopy(System.Type type, decimal darkEvoke, decimal glassCounter)
    {
        OrbModel copy = type == typeof(DarkOrb) ? ModelDb.Orb<DarkOrb>().ToMutable()
            : type == typeof(FrostOrb) ? ModelDb.Orb<FrostOrb>().ToMutable()
            : type == typeof(GlassOrb) ? ModelDb.Orb<GlassOrb>().ToMutable()
            : type == typeof(LightningOrb) ? ModelDb.Orb<LightningOrb>().ToMutable()
            : type == typeof(PlasmaOrb) ? ModelDb.Orb<PlasmaOrb>().ToMutable()
            : throw new System.ArgumentOutOfRangeException(nameof(type), type, "Unsupported orb type");
        if (copy is DarkOrb darkCopy)
        {
            DarkEvokeField.SetValue(darkCopy, darkEvoke);
        }
        if (copy is GlassOrb glassCopy)
        {
            GlassCounterField.SetValue(glassCopy, glassCounter);
        }
        return copy;
    }
}
