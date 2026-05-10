using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace wylder.Scripts.powers;

public class SacredBuffPower : BasicBuffPower
{
    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override string? CustomPackedIconPath => "res://wylder/powers/sacred_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/sacred_power.png";
    
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;
    
    //附魔回合结束减少一层
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        return;
    }
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("count", 2)];
    
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != Owner)
        {
            return 1m;
        }
        if (!props.IsPoweredAttack())
        {
            return 1m;
        }

        if (Owner.Player == null)
        {
            return 1m;
        }

        if (target != null)
        {
            bool shouldTriggerFatal = target.Powers.All((PowerModel p) => p.ShouldOwnerDeathTriggerFatal());
            if (!shouldTriggerFatal)
            {
                return 2m;
            }
        }
        return 1.2m;
    }
}