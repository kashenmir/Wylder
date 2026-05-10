using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using wylder.Scripts.cards;

namespace wylder.Scripts.powers;

public class FrostStrikePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override bool IsInstanced => true;
    
    public override int DisplayAmount => 3-DynamicVars["count"].IntValue;

    private bool isInActive = true;
    
    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override string? CustomPackedIconPath => "res://wylder/powers/basic_chase_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/basic_chase_power.png";

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("card", 3), new IntVar("count", 0), new IntVar("frost", 5)];
    
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player || isInActive)
        {
            isInActive = false;
            return;
        }
        DynamicVars["count"].UpgradeValueBy(1);
        if (DynamicVars["count"].IntValue >= DynamicVars["card"].IntValue)
        {
            Flash();
            List<Task> damageTasks = new List<Task>();
            foreach (Creature hittableEnemy in CombatState.HittableEnemies)
            {
                NCreature nCreature = NCombatRoom.Instance?.GetCreatureNode(hittableEnemy);
                if (nCreature != null)
                {
                    NGaseousImpactVfx child = NGaseousImpactVfx.Create(nCreature.VfxSpawnPosition, new Color("#F0F8FF"));
                    NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(child);
                    damageTasks.Add(DoDamage(context, [hittableEnemy], 1));
                    damageTasks.Add(DoPower([hittableEnemy], DynamicVars["frost"].IntValue));
                }
            }
            await Task.WhenAll(damageTasks);
            await PowerCmd.Remove(this);
        }
        InvokeDisplayAmountChanged();
    }
    
    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        if (cardSource == null || cardSource is not FrostStrike)
        {
            return;
        }

        if (cardSource.IsUpgraded)
        {
            DynamicVars["frost"].UpgradeValueBy(2);
            InvokeDisplayAmountChanged();
        }
    }
    
    private Task<IEnumerable<DamageResult>> DoDamage(PlayerChoiceContext choiceContext, IEnumerable<Creature> targets, int damage)
    {
        return CreatureCmd.Damage(choiceContext, targets, damage, ValueProp.Unpowered, Owner);
    }
    
    private Task<IReadOnlyList<FrostBase>> DoPower(IEnumerable<Creature> targets, int damage)
    {
        return PowerCmd.Apply<FrostBase>(targets, damage, Owner, null);
    }
}