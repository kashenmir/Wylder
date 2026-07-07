using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace wylder.Scripts.powers.recluse;

public class FreezingMistPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://wylder/powers/freezing_mist_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/freezing_mist_power.png";

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != Owner.Side || !Owner.IsAlive)
        {
            return;
        }

        List<Task> damageTasks = new List<Task>();
        foreach (Creature hittableEnemy in CombatState.HittableEnemies)
        {
            NCreature nCreature = NCombatRoom.Instance?.GetCreatureNode(hittableEnemy);
            if (nCreature != null)
            {
                NGaseousImpactVfx child = NGaseousImpactVfx.Create(nCreature.VfxSpawnPosition, new Color("#84A7A9"));
                NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(child);
                damageTasks.Add(PowerCmd.Apply<FrostBase>(new ThrowingPlayerChoiceContext(), hittableEnemy, Amount, Owner, null));
            }
        }
        await Task.WhenAll(damageTasks);
    }
}
