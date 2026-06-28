using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace wylder.Scripts.powers;

public class ScarletDewPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://wylder/powers/ScarletDewBubbletearRelic.png";
    public override string? CustomBigIconPath => "res://wylder/powers/ScarletDewBubbletearRelic.png";

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("heal", 2)];

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == CombatSide.Player)
        {
            Flash();
            await CreatureCmd.Heal(Owner, DynamicVars["heal"].IntValue);
            await PowerCmd.TickDownDuration(this);
        }
    }
}
