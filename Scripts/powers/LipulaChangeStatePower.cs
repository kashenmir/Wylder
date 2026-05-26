using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using wylder.Scripts.monsters;

namespace wylder.Scripts.powers;

public class LipulaChangeStatePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override string? CustomPackedIconPath => "res://wylder/powers/lipula_change_state_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/lipula_change_state_power.png";
    
    public override decimal ModifyHpLostBeforeOstyLate(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != base.Owner)
        {
            return amount;
        }
        if (amount == 0m)
        {
            return amount;
        }
        return Math.Min(amount, Math.Max(0,Owner.CurrentHp-Amount));
    }

    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (creature != base.Owner || Owner.CurrentHp > Amount)
        {
            return;
        }
        Flash();
        if (Owner.Monster == null)
        {
            throw new InvalidOperationException("Player cannot change state.");
        }
        if (CombatState != null && !Owner.IsDead)
        {
            base.Owner.Monster.SetMoveImmediate(Lipula._changeState, true);
        }
    }
}