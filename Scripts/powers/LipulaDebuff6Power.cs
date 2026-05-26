using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace wylder.Scripts.powers;

public class LipulaDebuff6Power : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;
    
    public override string? CustomPackedIconPath => "res://wylder/powers/lipula_debuff_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/lipula_debuff_power.png";
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("count", 1)];

    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (creature != Owner || creature.IsDead || DynamicVars["count"].IntValue<1)
        {
            return;
        }
        if (creature.CurrentHp <= creature.MaxHp * 0.2m)
        {
            await CreatureCmd.Heal(creature, creature.MaxHp*0.5m);
            DynamicVars["count"].UpgradeValueBy(-DynamicVars["count"].IntValue);
        }
    }

    public override decimal ModifyMaxEnergy(Player player, decimal amount)
    {
        if (player != base.Owner.Player)
        {
            return amount;
        }
        return amount - 1;
    }
}