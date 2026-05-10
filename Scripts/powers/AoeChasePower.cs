using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using wylder.Scripts.cards;

namespace wylder.Scripts.powers;

public class AoeChasePower : ChasePowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;
    
    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override string? CustomPackedIconPath => "res://wylder/powers/basic_chase_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/basic_chase_power.png";
    
    private bool isInActive = true;
    
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        return 0;
    }
    
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player || isInActive)
        {
            isInActive = false;
            return;
        }
        if (cardPlay.Card is not Suibu && cardPlay.Card is not HuntStep) {
            await PowerCmd.Remove(this);
        }
    }
    
    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        if (dealer == Owner && props.IsPoweredAttack() && cardSource!=null && cardSource.Tags.Contains(CardTag.Strike))
        {
            Flash();
            List<Task> damageTasks = new List<Task>();
            foreach (Creature hittableEnemy in CombatState.HittableEnemies)
            {
                if (hittableEnemy != target)
                { 
                    damageTasks.Add(DoDamage(choiceContext, [hittableEnemy], result.TotalDamage));
                }
            }
            await Task.WhenAll(damageTasks);
            await PowerCmd.Remove(this);
        }
    }
    
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == CombatSide.Enemy)
        {
            await PowerCmd.Remove(this);
        }
    }
    
    private Task<IEnumerable<DamageResult>> DoDamage(PlayerChoiceContext choiceContext, IEnumerable<Creature> targets, int damage)
    {
        return CreatureCmd.Damage(choiceContext, targets, damage, ValueProp.Move, Owner);
    }
}