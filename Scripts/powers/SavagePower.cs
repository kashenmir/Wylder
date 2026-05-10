using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace wylder.Scripts.powers;

public class SavagePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override string? CustomPackedIconPath => "res://wylder/powers/basic_chase_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/basic_chase_power.png";
    
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (!props.IsPoweredAttack() || target == null)
        {
            return 0m;
        }
        if (cardSource == null)
        {
            return 0m;
        }
        if (dealer != Owner)
        {
            return 0m;
        }
        int addValue = 0;
        foreach (PowerModel power in target.Powers.ToList())
        {
            if (power.Type == PowerType.Debuff)
            {
                addValue += Amount;
            }
        }
        return addValue;
    }
}