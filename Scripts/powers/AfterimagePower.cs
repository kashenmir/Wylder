using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace wylder.Scripts.powers;

public class AfterimagePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override string? CustomPackedIconPath => "res://wylder/powers/night_afterimage_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/night_afterimage_power.png";
    
    private class Data
    {
        public readonly Dictionary<CardModel, int> amountsForPlayedCards = new Dictionary<CardModel, int>();
    }
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Block)];

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        GetInternalData<Data>().amountsForPlayedCards.Add(cardPlay.Card, base.Amount);
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature.IsPlayer && cardPlay.Card.Owner.Creature.IsAlive && GetInternalData<Data>().amountsForPlayedCards.Remove(cardPlay.Card, out var value) && value > 0)
        {
            foreach (Creature enemy in CombatState.Enemies.ToList())
            {
                await CreatureCmd.GainBlock(enemy, value, ValueProp.Unpowered, null, fast: true);
            }
        }
    }
}