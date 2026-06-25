using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using wylder.Scripts.cards;

namespace wylder.Scripts.powers;

public class WeaponMasterPower : CustomPowerModel
{ 
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override string? CustomPackedIconPath => "res://wylder/powers/weapon_master_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/weapon_master_power.png";

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (Owner.Player == null || Owner.Player != player)
        {
            return;
        }
        for (int i = 0; i < base.Amount; i++)
        {
            List<CardPoolModel> list = base.Owner.Player.UnlockState.CharacterCardPools.ToList();
            IEnumerable<CardModel> cards1 = from c in list.SelectMany((CardPoolModel c) => c.GetUnlockedCards(base.Owner.Player.UnlockState, base.Owner.Player.RunState.CardMultiplayerConstraint))
                where c.Tags.Contains(CardTag.Strike) && c.Rarity!=CardRarity.Basic
                select c;
            IEnumerable<CardModel> cards2 = from c in list.SelectMany((CardPoolModel c) => c.GetUnlockedCards(base.Owner.Player.UnlockState, base.Owner.Player.RunState.CardMultiplayerConstraint))
                where c.Tags.Contains(CardTag.Strike) && c.Rarity==CardRarity.Basic
                select c;
            cards1 = cards1.Append(player.RunState.Rng.CombatCardGeneration.NextItem(cards2));
            CardModel? cardModel = CardFactory.GetDistinctForCombat(player, cards1, 1, player.RunState.Rng.CombatCardGeneration).FirstOrDefault();
            if (cardModel != null)
            {
                CardCmd.ApplyKeyword(cardModel, CardKeyword.Exhaust);
                cardModel.SetToFreeThisTurn();
                await CardPileCmd.AddGeneratedCardToCombat(cardModel, PileType.Hand, null);
            }
        }
    }   
}