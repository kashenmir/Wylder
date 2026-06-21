using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace wylder.Scripts.powers;

public class GoldenBackPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override string? CustomPackedIconPath => "res://wylder/powers/golden_defend_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/golden_defend_power.png";
    
    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (amount == 0m || power.GetTypeForAmount(amount) != PowerType.Debuff || power.Owner != Owner || applier == null || power is ITemporaryPower)
        {
            return;
        }
        Flash();
        List<Task> damageTasks = new List<Task>();
        foreach (Creature hittableEnemy in CombatState.HittableEnemies)
        {
            
            damageTasks.Add(DoDamage(choiceContext, [hittableEnemy], Amount));
        }
        await Task.WhenAll(damageTasks);
    }
    
    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult _, ValueProp props, Creature? dealer, CardModel? __)
    {
        if (target == base.Owner && dealer != null && props.IsPoweredAttack())
        {
            Flash();
            List<Task> damageTasks = new List<Task>();
            foreach (Creature hittableEnemy in CombatState.HittableEnemies)
            {
            
                damageTasks.Add(DoDamage(new ThrowingPlayerChoiceContext(), [hittableEnemy], Amount));
            }
            await Task.WhenAll(damageTasks);
        }
    }


    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == CombatSide.Enemy)
        {
            await PowerCmd.Remove(this);
        }
    }
    
    private Task<IEnumerable<DamageResult>> DoDamage(PlayerChoiceContext choiceContext, IEnumerable<Creature> targets, int damage)
    {
        return CreatureCmd.Damage(choiceContext, targets, damage, ValueProp.Unpowered, Owner);
    }
}