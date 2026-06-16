using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using wylder.Scripts.cards;

namespace wylder.Scripts.powers;

public class DemonEyePower : LipulaCurseBasePower
{
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, CombatState combatState)
    {
        if (player != base.Owner.Player)
        {
            return;
        }
        await CardPileCmd.AddToCombatAndPreview<Mad>(player.Creature, PileType.Hand, 1, addedByPlayer: false);
    }
}