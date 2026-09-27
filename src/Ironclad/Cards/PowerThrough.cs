using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace MySts2ContentMod.MySts2ContentModCode.Cards;

/// <summary>
/// 硬撑(Power Through):1费罕见技能牌。
/// 获得 15 点格挡(升级后 20),将 2 张伤口加入手牌。
/// </summary>
[Pool(typeof(IroncladCardPool))]
public sealed class PowerThrough : MySts2ContentModCard
{
    public override bool GainsBlock => true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[] { HoverTipFactory.FromCard<Wound>() };

    protected override IEnumerable<DynamicVar> CanonicalVars => new[] { new BlockVar(15m, ValueProp.Move) };

    public PowerThrough() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        await CardPileCmd.AddToCombatAndPreview<Wound>(base.Owner.Creature, PileType.Hand, 2, base.Owner);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(5m);
    }
}