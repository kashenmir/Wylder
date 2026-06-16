using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Afflictions;
using MegaCrit.Sts2.Core.Models.Powers;

namespace wylder.Scripts.powers;

public class CallOfVoidPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override string? CustomPackedIconPath => "res://wylder/powers/call_of_the_void_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/call_of_the_void_power.png";
    
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        List<CardModel> cards = PileType.Hand.GetPile(player).Cards.Where((CardModel c) => !c.Keywords.Contains(CardKeyword.Ethereal)).ToList().StableShuffle(player.RunState.Rng.CombatCardSelection)
            .Take(2)
            .ToList();
        if (cards.Count > 0)
        {
            foreach (CardModel allCard in cards)
            {
                await Afflict(allCard);
            }
        }
    }
    
    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Keywords.Contains(CardKeyword.Ethereal))
        {
            await PowerCmd.Apply<VigorPower>(Owner, Amount, Owner, null);
        }
    }
    
    private async Task Afflict(CardModel card)
    {
        if (card.Affliction == null)
        {
            Hexed? hexed = await CardCmd.Afflict<Hexed>(card, 1);
            if (hexed != null && !card.Keywords.Contains(CardKeyword.Ethereal))
            {
                CardCmd.ApplyKeyword(card, CardKeyword.Ethereal);
                hexed.AppliedEthereal = true;
            }
        }
    }
}