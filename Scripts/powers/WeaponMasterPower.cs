using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using wylder.Scripts.cards;

namespace wylder.Scripts.powers;

public class WeaponMasterPower : CustomPowerModel
{
    private class Data
    {
        public int etherealCount;
    }    
    
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override string? CustomPackedIconPath => "res://wylder/powers/weapon_master_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/weapon_master_power.png";
    
    protected override object InitInternalData()
    {
        return new Data();
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if (card.Owner.Creature == base.Owner)
        {
            if (causedByEthereal)
            {
                GetInternalData<Data>().etherealCount++;
            }
            else
            {
                List<CardModel> list = CardFactory.GetForCombat(base.Owner.Player, from c in base.Owner.Player.Character.CardPool.GetUnlockedCards(base.Owner.Player.UnlockState, base.Owner.Player.RunState.CardMultiplayerConstraint)
                    where c is AshWarModel
                    select c, base.Amount, base.Owner.Player.RunState.Rng.CombatCardGeneration).ToList();
                foreach (CardModel item in list)
                {
                    await CardPileCmd.AddGeneratedCardToCombat(item, PileType.Hand, addedByPlayer: true);
                }
            }
        }
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == CombatSide.Player)
        {
            Data data = GetInternalData<Data>();
            if (data.etherealCount > 0)
            {
                List<CardModel> list = CardFactory.GetForCombat(base.Owner.Player,
                        from c in base.Owner.Player.Character.CardPool.GetUnlockedCards(base.Owner.Player.UnlockState,
                            base.Owner.Player.RunState.CardMultiplayerConstraint)
                        where c is AshWarModel
                        select c, base.Amount * data.etherealCount, base.Owner.Player.RunState.Rng.CombatCardGeneration)
                    .ToList();
                foreach (CardModel item in list)
                {
                    await CardPileCmd.AddGeneratedCardToCombat(item, PileType.Hand, addedByPlayer: true);
                }
                data.etherealCount = 0;
            }
        }
    }
}