using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace wylder.Scripts.powers.recluse;

public class LawOfCausalityPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public override int DisplayAmount => DynamicVars["count"].IntValue;

    public override string? CustomPackedIconPath => "res://wylder/powers/law_of_causality_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/law_of_causality_power.png";

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("threshold", 20), new IntVar("damage", 20), new IntVar("count", 0)];

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || !props.IsPoweredAttack())
        {
            return;
        }
        DynamicVars["count"].UpgradeValueBy(result.UnblockedDamage);
        if (DynamicVars["count"].IntValue >= DynamicVars["threshold"].IntValue)
        {
            DynamicVars["count"].UpgradeValueBy(-DynamicVars["count"].IntValue);
            Flash();
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), CombatState.HittableEnemies, DynamicVars["damage"].IntValue, ValueProp.Unpowered, Owner);
        }
        InvokeDisplayAmountChanged();
    }
}
