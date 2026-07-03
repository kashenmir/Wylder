using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace wylder.Scripts.powers;

public class Frost : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override string? CustomPackedIconPath => "res://wylder/powers/frost.png";
    public override string? CustomBigIconPath => "res://wylder/powers/frost.png";
    
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (target != Owner || !props.IsPoweredAttack())
        {
            return 1m;
        }
        return 1.25m;
    }
    
    public override bool TryModifyPowerAmountReceived(PowerModel canonicalPower, Creature target, decimal amount, Creature? applier, out decimal modifiedAmount)
    {
        // 1. 先假设我们不拦截，数值保持不变
        modifiedAmount = amount;
        
        if (target != Owner)
        {
            modifiedAmount = amount;
            return false; // 不拦截，使用原本的数值
        }

        // 2. 安全检查：确保 canonicalPower 是 FrostbitePower 的原型
        //    并且当前正在处理的 target (怪物) 身上确实有 HeatPower
        if (canonicalPower is FrostBase 
            && target.HasPower<Frost>()) // <-- 这里换成你实际的“另一种状态”类名
        {
            // 3. 拦截逻辑：把要增加的层数强制归零
            modifiedAmount = 0;

            // 4. 返回 true，告诉系统："我修改了数值，请用我修改后的 (0) "
            return true;
        }

        // 5. 如果没有 HeatPower，返回 false，表示不修改数值，正常施加
        return false;
    }

    //公共方法，移除冰霜效果
    public async Task removeFrost(Creature target)
    {
        Frost? frost = target.GetPower<Frost>();
        if (frost != null)
        {
            await PowerCmd.Remove(frost);
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == CombatSide.Enemy)
        {
            await PowerCmd.TickDownDuration(this);
        }
    }
}