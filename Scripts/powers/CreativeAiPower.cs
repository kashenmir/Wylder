using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace wylder.Scripts.powers;

public class CreativeAiPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override string? CustomPackedIconPath => "res://wylder/powers/creative_ai_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/creative_ai_power.png";

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        for (int i = 0; i < base.Amount; i++)
        {
            CardModel? cardModel = CardFactory.GetDistinctForCombat(player, from c in ModelDb.CardPool<StatusCardPool>().GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint)
                where c.Type == CardType.Status
                select c, 1, player.RunState.Rng.CombatCardGeneration).FirstOrDefault();
            if (cardModel != null)
            {
                await CardPileCmd.AddGeneratedCardToCombat(cardModel, PileType.Hand, null);
            }
        }
    }
}