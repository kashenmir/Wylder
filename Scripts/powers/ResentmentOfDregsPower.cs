using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace wylder.Scripts.powers;

public class ResentmentOfDregsPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override string? CustomPackedIconPath => "res://wylder/powers/resentment_of_dregs_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/resentment_of_dregs_power.png";
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<RotBase>(),HoverTipFactory.FromPower<Rot>()];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
        {
            return;
        }
        int cardsCount = PileType.Exhaust.GetPile(Owner.Player).Cards.Count;
        if (cardsCount <= 0)
        {
            return;
        }
        Flash();
        await Cmd.CustomScaledWait(0.2f, 0.4f);
        List<Task> damageTasks = new List<Task>();
        foreach (Creature hittableEnemy in CombatState.HittableEnemies)
        {
            NCreature nCreature = NCombatRoom.Instance?.GetCreatureNode(hittableEnemy);
            if (nCreature != null)
            {
                NGaseousImpactVfx child = NGaseousImpactVfx.Create(nCreature.VfxSpawnPosition, new Color("#8B0000"));
                NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(child);
                Rot? rot = hittableEnemy.GetPower<Rot>();
                if (rot != null)
                {
                    damageTasks.Add(DoDamage(choiceContext, [hittableEnemy], cardsCount*Amount));
                }
                else
                {
                    damageTasks.Add(DoPower([hittableEnemy], cardsCount*Amount));
                }
            }
        }
        await Task.WhenAll(damageTasks);
    }
    
    private Task<IEnumerable<DamageResult>> DoDamage(PlayerChoiceContext choiceContext, IEnumerable<Creature> targets, int damage)
    {
        return CreatureCmd.Damage(choiceContext, targets, damage, ValueProp.Unpowered, Owner);
    }
    
    private Task<IReadOnlyList<RotBase>> DoPower(IEnumerable<Creature> targets, int damage)
    {
        return PowerCmd.Apply<RotBase>(targets, damage, Owner, null);
    }
}