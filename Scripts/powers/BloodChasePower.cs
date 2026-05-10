using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using wylder.Scripts.cards;

namespace wylder.Scripts.powers;

public class BloodChasePower : ChasePowerModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("blood", 5m)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<BloodBase>()];
    
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player || isInActive)
        {
            isInActive = false;
            return;
        }

        // if (cardPlay.Card.Tags.Contains(CardTag.Strike))
        // {
        //     await CardPileCmd.Draw(context, Amount, Owner.Player);
        //     await PowerCmd.Apply<BloodBase>(cardPlay.Target, DynamicVars["blood"].IntValue, Owner, null);
        // }

        if (cardPlay.Card is not Suibu && cardPlay.Card is not HuntStep) {
            await PowerCmd.Remove(this);
        }
    }
    
    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        if (dealer == Owner && props.IsPoweredAttack() && cardSource!=null && cardSource.Tags.Contains(CardTag.Strike))
        {
            Flash();
            await CardPileCmd.Draw(choiceContext, Amount, Owner.Player);
            await PowerCmd.Apply<BloodBase>(target, DynamicVars["blood"].IntValue, Owner, null);
            await PowerCmd.Remove(this);
        }
    }
}