using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using DefectRebalance.DefectRebalanceCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;

namespace DefectRebalance.DefectRebalanceCode.Powers;

/// <summary>
/// 散热片(HeatSinks)能力:你每打出一张能力牌,抽 base.Amount 张牌。
/// </summary>
public class HeatSinksPower : DefectRebalancePower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => new PowerLoc(
        "散热片",
        "每当你打出一张[gold]能力牌[/gold]，抽{Amount}张牌。",
        "每当你打出一张[gold]能力牌[/gold]，抽{Amount}张牌。");

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Type == CardType.Power && cardPlay.Player == base.Owner!.Player)
        {
            int amount = base.Amount;
            if (cardPlay.Card is HeatSinks heatSinks)
            {
                amount -= heatSinks.IsUpgraded ? 2 : 1;
            }
            if (amount > 0)
            {
                await CardPileCmd.Draw(choiceContext, amount, cardPlay.Player);
            }
        }
    }
}
