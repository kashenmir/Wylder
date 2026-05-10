using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace wylder.Scripts.powers;

public class SilverTearPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override string? CustomPackedIconPath => "res://wylder/powers/silver_tear_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/silver_tear_power.png";
    
    
    public override bool TryModifyPowerAmountReceived(PowerModel canonicalPower, Creature target, decimal amount, Creature? applier, out decimal modifiedAmount)
    {
        // 1. 先假设我们不拦截，数值保持不变
        modifiedAmount = amount;
        
        if (!target.IsEnemy)
        {
            modifiedAmount = amount;
            return false; // 不拦截，使用原本的数值
        }

        // 2. 安全检查：确保 canonicalPower 是 FrostbitePower 的原型
        //    并且当前正在处理的 target (怪物) 身上确实有 HeatPower
        if (checkPower(canonicalPower, target))
        {
            modifiedAmount = amount+Amount;
            return true;
        }

        // 5. 如果没有 HeatPower，返回 false，表示不修改数值，正常施加
        return false;
    }

    public bool checkPower(PowerModel powerModel, Creature target)
    {
        return (powerModel is RotBase && !target.HasPower<Rot>()) ||
               (powerModel is PoisonBase && !target.HasPower<Poison>()) ||
               (powerModel is FrostBase && !target.HasPower<Frost>()) ||
               (powerModel is SleepBase && !target.HasPower<Sleep>()) ||
               (powerModel is BloodBase) || (powerModel is MadnessBase);
    }
}