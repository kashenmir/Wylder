using BaseLib.Abstracts;
using BaseLib.Hooks;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace wylder.Scripts.powers;

public class Rot : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override Color AmountLabelColor => _normalAmountLabelColor;
    
    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override string? CustomPackedIconPath => "res://wylder/powers/rot.png";
    public override string? CustomBigIconPath => "res://wylder/powers/rot.png";
    
    
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
        if (canonicalPower is RotBase 
            && target.HasPower<Rot>()) // <-- 这里换成你实际的“另一种状态”类名
        {
            // 3. 拦截逻辑：把要增加的层数强制归零
            modifiedAmount = 0;

            // 4. 返回 true，告诉系统："我修改了数值，请用我修改后的 (0) "
            return true;
        }

        // 5. 如果没有 HeatPower，返回 false，表示不修改数值，正常施加
        return false;
    }
    // 1. 核心逻辑：回合开始造成伤害
    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != Owner.Side || !Owner.IsAlive)
        {
            return;
        }

        // 造成固定 5 点伤害
        await CreatureCmd.Damage(
            new ThrowingPlayerChoiceContext(), 
            base.Owner, 
            (int)(10m+Owner.MaxHp*0.03m), 
            ValueProp.Unblockable | ValueProp.Unpowered, 
            null, 
            null
        );

        // 造成伤害后，消耗 1 层
        await PowerCmd.Decrement(this);
    }

    public override IEnumerable<HealthBarForecastSegment> GetHealthBarForecastSegments(
        HealthBarForecastContext context)
    {
        HealthBarForecastSegment segment = new HealthBarForecastSegment((int)(10m+Owner.MaxHp*0.03m), new Color("#7E4743"), HealthBarForecastDirection.FromRight, 3);
        return [segment];
    }
}