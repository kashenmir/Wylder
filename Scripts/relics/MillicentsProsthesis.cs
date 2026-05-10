using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using wylder.Scripts.pools;

namespace wylder.Scripts.relics;

// 加入哪个遗物池，此处为通用
[Pool(typeof(WylderRelicPool))]
public class MillicentsProsthesis : CustomRelicModel
{
    // 稀有度
    public override RelicRarity Rarity => RelicRarity.Rare;
    
    public override bool ShowCounter => true;
    
    // 小图标（原版85x85）
    public override string PackedIconPath => $"res://wylder/images/relics/{GetType().Name}.png";
    // 轮廓图标（原版85x85）
    protected override string PackedIconOutlinePath => $"res://wylder/images/relics/{GetType().Name}.png";
    // 大图标（原版256x256）
    protected override string BigIconPath => $"res://wylder/images/relics/{GetType().Name}.png";
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DexterityPower>()];

    private int AttackPlayed = 0;
    
    public override int DisplayAmount
    {
        get
        {
            return AttackPlayed;
        }
    }
    
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (!props.IsPoweredAttack())
        {
            return 1m;
        }
        if (cardSource == null)
        {
            return 1m;
        }
        if (cardSource.Owner.Creature != Owner.Creature)
        {
            return 1m;
        }

        if (AttackPlayed > 20)
        {
            return 1.5m;
        } else if (AttackPlayed > 15)
        {
            return 1.3m;
        } else if (AttackPlayed > 10)
        {
            return 1.2m;
        } else if (AttackPlayed > 5)
        {
            return 1.1m;
        }

        return 1m;
    }
    
    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner == base.Owner && cardPlay.Card.Type != CardType.Attack)
        {
            AttackPlayed -= 1;
            InvokeDisplayAmountChanged();
        }
        return Task.CompletedTask;
    }
    
    public override Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        if (dealer == Owner.Creature && props.IsPoweredAttack())
        {
            AttackPlayed += 2;
            InvokeDisplayAmountChanged();
        }
        return Task.CompletedTask;
    }
    
    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is CombatRoom)
        {
            Flash();
            AttackPlayed = 0;
            await PowerCmd.Apply<DexterityPower>(Owner.Creature, 1, Owner.Creature, null);
            InvokeDisplayAmountChanged();
        }
    }
    
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == CombatSide.Player)
        {
            if (AttackPlayed > 20)
            {
                AttackPlayed -= 5;
            } else if (AttackPlayed > 0)
            {
                AttackPlayed -= 1;
            }
            InvokeDisplayAmountChanged();
        }
    }
}