using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace wylder.Scripts.powers;

public class FalseBlessingPower : LipulaCurseBasePower
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("hpLoss", 2)];

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (ShouldSkip(card))
        {
            return false;
        }
        modifiedCost = default(decimal);
        return true;
    }

    public override bool TryModifyStarCost(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (ShouldSkip(card))
        {
            return false;
        }
        modifiedCost = default(decimal);
        return true;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature == Owner && cardPlay != null && cardPlay.Card.Type == CardType.Skill)
        {
            Flash();
            await CreatureCmd.LoseMaxHp(context, Owner, DynamicVars["hpLoss"].IntValue, false);
        }
    }

    private bool ShouldSkip(CardModel card)
    {
        if (card.Owner.Creature != Owner)
        {
            return true;
        }
        if (card.Pile?.Type is not (PileType.Hand or PileType.Play))
        {
            return true;
        }
        if (card.Type != CardType.Skill)
        {
            return true;
        }
        return false;
    }
}
