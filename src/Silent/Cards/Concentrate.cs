using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace SilentRebalance.SilentRebalanceCode.Cards;

[Pool(typeof(SilentCardPool))]
public sealed class Concentrate : SilentRebalanceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CardsVar(3),
        new EnergyVar(2)
    };

    public Concentrate() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int cardCount = base.IsUpgraded ? 2 : 3;
        CardSelectorPrefs prefs = new(CardSelectorPrefs.DiscardSelectionPrompt, cardCount);
        IEnumerable<CardModel> selected = await CardSelectCmd.FromHandForDiscard(
            choiceContext, base.Owner, prefs, null, this);
        await CardCmd.Discard(choiceContext, selected);
        await PlayerCmd.GainEnergy(2m, base.Owner);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Cards.UpgradeValueBy(-1m);
    }
}
