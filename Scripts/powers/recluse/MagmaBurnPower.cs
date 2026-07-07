using BaseLib.Abstracts;
using BaseLib.Hooks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using Godot;

namespace wylder.Scripts.powers.recluse;

public class MagmaBurnPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://wylder/powers/magma_burn_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/magma_burn_power.png";

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != Owner.Side || !Owner.IsAlive)
        {
            return;
        }

        await CreatureCmd.Damage(
            new ThrowingPlayerChoiceContext(),
            Owner,
            8,
            ValueProp.Unblockable | ValueProp.Unpowered,
            null,
            null
        );

        await PowerCmd.Decrement(this);
    }
    
    public override IEnumerable<HealthBarForecastSegment> GetHealthBarForecastSegments(
        HealthBarForecastContext context)
    {
        HealthBarForecastSegment segment = new HealthBarForecastSegment(8, new Color("#FF4500"), HealthBarForecastDirection.FromRight, 1);
        return [segment];
    }
}
