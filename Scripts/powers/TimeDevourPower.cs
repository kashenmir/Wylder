using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace wylder.Scripts.powers;

public class TimeDevourPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override string? CustomPackedIconPath => "res://wylder/powers/time_eater_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/time_eater_power.png";

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("clockAmount", 1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<ClockCountPower>()];

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        await ApplyClockToAllPlayers();
    }

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side == Owner.Side)
        {
            await ApplyClockToAllPlayers();
        }
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target == Owner && Owner.IsDead)
        {
            await OnDeathEffect();
        }
    }

    public override async Task AfterCombatEnd(CombatRoom _)
    {
        await RemoveAllClockFromPlayers();
    }

    private async Task ApplyClockToAllPlayers()
    {
        foreach (Creature creature in CombatState.HittableEnemies.ToList())
        {
            if (creature.IsPlayer && creature.IsAlive)
            {
                await PowerCmd.Apply<ClockCountPower>(new ThrowingPlayerChoiceContext(), creature, DynamicVars["clockAmount"].IntValue, Owner, null);
            }
        }
    }

    private async Task OnDeathEffect()
    {
        await RemoveAllClockFromPlayers();
        foreach (Creature creature in CombatState.HittableEnemies.ToList())
        {
            if (creature.IsPlayer && creature.IsAlive && creature.Player != null)
            {
                PlayerCmd.EndTurn(creature.Player, canBackOut: false);
            }
        }
    }

    private async Task RemoveAllClockFromPlayers()
    {
        foreach (Creature creature in CombatState.HittableEnemies.ToList())
        {
            if (creature.IsPlayer)
            {
                ClockCountPower? clock = creature.GetPower<ClockCountPower>();
                if (clock != null)
                {
                    await PowerCmd.Remove(clock);
                }
            }
        }
    }
}
