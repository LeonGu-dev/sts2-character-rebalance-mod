using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using DefectRebalance.DefectRebalanceCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;

namespace DefectRebalance.DefectRebalanceCode.Cards;

/// <summary>
/// 散热片(HeatSinks,STS1回归,新增卡)。
/// 1费罕见能力:每当你打出一张能力牌,抽1(升2)张牌。
/// </summary>
[Pool(typeof(DefectCardPool))]
public sealed class HeatSinks : DefectRebalanceCard
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[] { HoverTipFactory.FromPower<HeatSinksPower>() };

    protected override IEnumerable<DynamicVar> CanonicalVars => new[] { new PowerVar<HeatSinksPower>(1m) };

    public HeatSinks() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(base.Owner!.Creature, "PowerUp", base.Owner.Character.PowerUpAnimDelay);
        await PowerCmd.Apply<HeatSinksPower>(choiceContext, base.Owner.Creature, base.DynamicVars["HeatSinksPower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["HeatSinksPower"].UpgradeValueBy(1m);
    }
}
