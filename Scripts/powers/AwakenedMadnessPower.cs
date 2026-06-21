using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using wylder.Scripts.cards;

namespace wylder.Scripts.powers;

public class AwakenedMadnessPower : LipulaCurseBasePower
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("madness", 2)];

    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        if (dealer == Owner && props.IsPoweredAttack() && result.UnblockedDamage > 0)
        {
            Flash();
            await PowerCmd.Apply<MadnessBase>(choiceContext, target, DynamicVars["madness"].IntValue, Owner, cardSource);
        }
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target == Owner && props.IsPoweredAttack() && result.UnblockedDamage > 0)
        {
            Flash();
            await CardPileCmd.AddToCombatAndPreview<Mad>([Owner], PileType.Discard, 1, null);
        }
    }
}
