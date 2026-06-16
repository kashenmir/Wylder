using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace wylder.Scripts.powers;

public class OrbitPower : CustomPowerModel
{
    private class Data
    {
        public int energySpent;

        public int triggerCount;
    }

    private const int _energyIncrement = 4;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override string? CustomPackedIconPath => "res://wylder/powers/night_orbit_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/night_orbit_power.png";

    public override int DisplayAmount => 4 - GetInternalData<Data>().energySpent % 4;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.ForEnergy(this)];

    public override bool IsInstanced => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(4)];

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override async Task AfterEnergySpent(CardModel card, int amount)
    {
        if (amount > 0)
        {
            Data data = GetInternalData<Data>();
            data.energySpent += amount;
            int triggers = data.energySpent / 4 - data.triggerCount;
            if (triggers > 0)
            {
                Flash();
                await PlayerCmd.LoseEnergy(base.Amount * triggers, card.Owner);
                await PowerCmd.Apply<VigorPower>(Owner, 4*triggers, Owner, null);
                data.triggerCount += triggers;
            }
            InvokeDisplayAmountChanged();
        }
    }
}