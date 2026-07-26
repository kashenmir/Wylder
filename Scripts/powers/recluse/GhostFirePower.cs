using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace wylder.Scripts.powers.recluse;

public class GhostFirePower : CustomPowerModel
{
    private const string _durationKey = "Duration";

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public override int DisplayAmount => DynamicVars[_durationKey].IntValue;

    public override string? CustomPackedIconPath => "res://wylder/powers/ghost_fire_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/ghost_fire_power.png";

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar(_durationKey, 4m)];

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == CombatSide.Player)
        {
            await CreatureCmd.Damage(choiceContext,
                CombatState.HittableEnemies, Amount, ValueProp.Unpowered,
                base.Owner);

            DynamicVars[_durationKey].BaseValue -= 1m;
            InvokeDisplayAmountChanged();

            if (DynamicVars[_durationKey].BaseValue <= 0m)
            {
                await PowerCmd.Remove(this);
            }
        }
    }
}
