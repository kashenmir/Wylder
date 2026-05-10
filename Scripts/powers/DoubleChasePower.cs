using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace wylder.Scripts.powers;

public class DoubleChasePower : ChasePowerModel
{
    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
    {
        if (card.Owner.Creature != base.Owner || !card.Tags.Contains(CardTag.Strike))
        {
            return playCount;
        }
        return playCount + Amount;
    }

    public override async Task AfterModifyingCardPlayCount(CardModel card)
    {
        Flash();
        await PowerCmd.Remove(this);
    }
}