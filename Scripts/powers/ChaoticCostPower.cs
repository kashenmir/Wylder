using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace wylder.Scripts.powers;

public class ChaoticCostPower : LipulaCurseBasePower
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("maxCost", 4)];

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Owner != Owner.Player)
        {
            return;
        }
        if (card.EnergyCost.Canonical < 0)
        {
            return;
        }
        int cost = Owner.Player.RunState.Rng.CombatEnergyCosts.NextInt(DynamicVars["maxCost"].IntValue);
        card.EnergyCost.SetThisCombat(cost);
        NCard.FindOnTable(card)?.PlayRandomizeCostAnim();
    }
}
