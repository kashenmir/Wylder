using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace wylder.Scripts.powers;

public class ChaseMasterPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override string? CustomPackedIconPath => "res://wylder/powers/chase_master_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/chase_master_power.png";
    
    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != base.Owner)
        {
            return;
        }
        if (cardPlay.Card.Tags.Contains(CardTag.Strike))
        {
            foreach (PowerModel power in Owner.Powers)
            {
                if (power is ChasePowerModel)
                {
                    Flash();
                    await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null, false);
                    return;
                }
            }
            await CreatureCmd.GainBlock(Owner, 1, ValueProp.Unpowered, null, false);
            return;
        }
        return;
    }

}