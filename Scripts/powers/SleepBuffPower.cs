using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace wylder.Scripts.powers;

public class SleepBuffPower : BasicBuffPower
{
    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override string? CustomPackedIconPath => "res://wylder/powers/sleep_grease_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/sleep_grease_power.png";
    
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;
    
    //附魔回合结束减少一层
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        return;
    }
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("count", 2)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<SleepBase>(), HoverTipFactory.FromPower<Sleep>()];
    
    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        if (dealer == Owner && props.IsPoweredAttack())
        {
            await PowerCmd.Apply<SleepBase>(target, DynamicVars["count"].IntValue, Owner, null);
        }
    }
}