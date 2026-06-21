using BaseLib.Abstracts;
using BaseLib.Hooks;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.ValueProps;

namespace wylder.Scripts.powers;

public class DeathFlamePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override string? CustomPackedIconPath => "res://wylder/powers/death_flame_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/death_flame_power.png";
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<FrostBase>(),HoverTipFactory.FromPower<Frost>()];
    
    // 1. 核心逻辑：回合开始造成伤害
    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side != Owner.Side || !Owner.IsAlive)
        {
            return;
        }

        // 造成固定 5 点伤害
        await CreatureCmd.Damage(
            new ThrowingPlayerChoiceContext(), 
            base.Owner, 
            8, 
            ValueProp.Unblockable | ValueProp.Unpowered, 
            null, 
            null
        );

        await PowerCmd.Apply<FrostBase>(new ThrowingPlayerChoiceContext(), Owner, 8, Owner, null);

        // 造成伤害后，消耗 1 层
        await PowerCmd.Decrement(this);
    }
    
    public override IEnumerable<HealthBarForecastSegment> GetHealthBarForecastSegments(
        HealthBarForecastContext context)
    {
        HealthBarForecastSegment segment = new HealthBarForecastSegment(8, new Color("#84A7A9"), HealthBarForecastDirection.FromRight, 1);
        return [segment];
    }
}