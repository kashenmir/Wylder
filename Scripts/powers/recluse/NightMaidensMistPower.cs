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

public class NightMaidensMistPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://wylder/powers/night_maidens_mist_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/night_maidens_mist_power.png";

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != Owner.Side || !Owner.IsAlive)
        {
            return;
        }

        Flash();

        await CreatureCmd.Damage(
            new ThrowingPlayerChoiceContext(),
            Owner,
            Amount,
            ValueProp.Unblockable | ValueProp.Unpowered,
            null,
            null
        );

        await PowerCmd.Apply<NightMaidensMistPower>(new ThrowingPlayerChoiceContext(), Owner, 2, Owner, null);
    }

    public override IEnumerable<HealthBarForecastSegment> GetHealthBarForecastSegments(HealthBarForecastContext context)
    {
        HealthBarForecastSegment segment = new HealthBarForecastSegment(Amount, new Color("#00008B"), HealthBarForecastDirection.FromRight, 1);
        return [segment];
    }
}
