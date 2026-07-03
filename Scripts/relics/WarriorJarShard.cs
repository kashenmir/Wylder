using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using wylder.Scripts.pools;

namespace wylder.Scripts.relics;

// 加入哪个遗物池，此处为通用
[Pool(typeof(WylderRelicPool))]
public class WarriorJarShard : CustomRelicModel
{
    // 稀有度
    public override RelicRarity Rarity => RelicRarity.Rare;
    
    public override bool ShowCounter => false;
    
    // 小图标（原版85x85）
    public override string PackedIconPath => $"res://wylder/images/relics/{GetType().Name}.png";
    // 轮廓图标（原版85x85）
    protected override string PackedIconOutlinePath => $"res://wylder/images/relics/{GetType().Name}.png";
    // 大图标（原版256x256）
    protected override string BigIconPath => $"res://wylder/images/relics/{GetType().Name}.png";
    
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
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
        int num = CombatManager.Instance.History.CardPlaysStarted.Count((CardPlayStartedEntry e) => e.HappenedThisTurn(base.Owner.Creature.CombatState) && e.CardPlay.Card.Type == CardType.Attack && e.CardPlay.Card.Owner.Creature == base.Owner.Creature);
        CardPile pile = cardSource.Pile;
        int num2 = ((pile != null && pile.Type == PileType.Play) ? 1 : 0);
        if (num > num2)
        {
            return 1m;
        }
        return 1.5m;
    }

    public override Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        Status = RelicStatus.Active;
        return Task.CompletedTask;
    }
    
    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner == base.Owner && cardPlay.Card.Type == CardType.Attack)
        {
            Status = RelicStatus.Normal;
        }
        return Task.CompletedTask;
    }
}