using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace wylder.Scripts.powers;

public class CoolantPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://wylder/powers/coolant_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/coolant_power.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<BufferPower>()];

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side == base.Owner.Side)
        {
            Flash();
            foreach (Creature creature in combatState.Enemies)
            {
                BufferPower? bufferPower = creature.GetPower<BufferPower>();
                if (bufferPower == null)
                {
                    await PowerCmd.Apply<BufferPower>(new ThrowingPlayerChoiceContext(), creature, Amount, Owner, null);
                } else if (bufferPower.Amount < Amount)
                {
                    await PowerCmd.Apply<BufferPower>(new ThrowingPlayerChoiceContext(), creature, Amount-bufferPower.Amount, Owner, null);
                }
            }
        }
    }    
}