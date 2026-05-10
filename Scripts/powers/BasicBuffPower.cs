using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace wylder.Scripts.powers;

public class BasicBuffPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    //附魔回合结束减少一层
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == CombatSide.Enemy)
        {
            await PowerCmd.TickDownDuration(this);
        }
    }
    
    //施加后移除其他附魔
    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        if (Owner.Player != null && Owner.GetPowerInstances<BasicBuffPower>() != null)
        {
            foreach (BasicBuffPower item in Owner.GetPowerInstances<BasicBuffPower>().ToList())
            {
                if (item != this)
                {
                    await PowerCmd.Remove(item);
                }
            }
        }
    }
}