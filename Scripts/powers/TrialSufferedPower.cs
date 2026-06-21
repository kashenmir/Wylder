using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace wylder.Scripts.powers;

public class TrialSufferedPower : LipulaCurseBasePower
{
    private int _cardsPlayedThisTurn;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("limit", 4)];

    public override int DisplayAmount => _cardsPlayedThisTurn;

    public override bool ShouldPlay(CardModel card, AutoPlayType _)
    {
        if (card.Owner.Creature != Owner)
        {
            return true;
        }
        return _cardsPlayedThisTurn < DynamicVars["limit"].IntValue;
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player)
        {
            return Task.CompletedTask;
        }
        _cardsPlayedThisTurn++;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != Owner.Side)
        {
            return Task.CompletedTask;
        }
        _cardsPlayedThisTurn = 0;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
}
