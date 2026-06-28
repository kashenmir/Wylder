using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace wylder.Scripts.powers;

public class MentalFortressPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://wylder/powers/mental_fortress_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/mental_fortress_power.png";

    public override bool TryModifyPowerAmountReceived(PowerModel canonicalPower, Creature target, decimal amount, Creature? applier, out decimal modifiedAmount)
    {
        modifiedAmount = amount;

        if (canonicalPower.StackType != PowerStackType.Counter || amount <= 0 || target != Owner)
        {
            return false;
        }

        if (canonicalPower.Type == PowerType.Buff)
        {
            modifiedAmount = amount * 2m;
            Flash();
            return true;
        }

        if (canonicalPower.Type == PowerType.Debuff)
        {
            modifiedAmount = Math.Floor(amount * 0.5m);
            Flash();
            return true;
        }

        return false;
    }

    public override async Task AfterModifyingPowerAmountReceived(PowerModel power)
    {
        if (power.Owner != Owner)
        {
            return;
        }
        await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null);
    }
}
