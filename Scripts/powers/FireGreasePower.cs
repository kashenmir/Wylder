using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace wylder.Scripts.powers;

public class FireGreasePower : BasicBuffPower
{
    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override string? CustomPackedIconPath => "res://wylder/powers/fire_grease_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/fire_grease_power.png";
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(2, ValueProp.Move)];
    
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (!props.IsPoweredAttack())
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
        return DynamicVars.Damage.IntValue;
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        if (dealer == Owner && props.IsPoweredAttack())
        {
            Frost? frost = target.GetPower<Frost>();
            if (frost != null)
            {
                await PowerCmd.Remove(frost);
            }
        }
    }
}