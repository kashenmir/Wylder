using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Combat;
using DemonFormPower = wylder.Scripts.powers.DemonFormPower;

namespace wylder.Scripts.monsters;

public class NightIronclad : CustomMonsterModel
{
    // 根据进阶提高最小血量，进阶8及以上为120，否则为100
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 160, 150);

    // 根据进阶提高最大血量，进阶8及以上为140，否则为120
    public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 180, 160);

    // 怪物场景，如果你的场景没有挂载脚本，参考这个
    public override NCreatureVisuals? CreateCustomVisuals() => NodeFactory<NCreatureVisuals>.CreateFromScene("res://wylder/scenes/ironclad/ironclad.tscn");

    private int HeavyDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 13, 12);
    
    private int DoubleDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 5, 4);

    private int MultiHits => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 5, 4);
    
    // 战斗开始时，在这里给自己上buff之类
    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
        await PowerCmd.Apply<IntangiblePower>(Creature, 2, Creature, null);
        await PowerCmd.Apply<DemonFormPower>(Creature, 3, Creature, null);
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var bash = new MoveState(
            "BASH",
            async targets =>
            {
                //await CreatureCmd.TriggerAnim(base.Creature, "Attack", 0.5f);
                await DamageCmd 
                    .Attack(HeavyDamage)
                    .FromMonster(this)
                    .WithHitFx("vfx/vfx_attack_blunt")
                    .Execute(null);
                await PowerCmd.Apply<VulnerablePower>(targets, 3m, base.Creature, null);
            }, new SingleAttackIntent(HeavyDamage), new DebuffIntent()
        );
        
        var twinStrike = new MoveState(
            "TWIN_STRIKE",
            async targets =>
            {
                //await CreatureCmd.TriggerAnim(base.Creature, "Attack", 0.5f);
                await DamageCmd 
                    .Attack(DoubleDamage)
                    .WithHitCount(2)
                    .FromMonster(this)
                    .WithHitFx("vfx/vfx_attack_slash")
                    .Execute(null);
            }, new MultiAttackIntent(DoubleDamage, 2)
        );
        
        var conflagration = new MoveState(
            "CONFLAGRATION",
            async targets =>
            {
                //await CreatureCmd.TriggerAnim(base.Creature, "Attack", 0.5f);
                await DamageCmd 
                    .Attack(2)
                    .WithHitCount(MultiHits)
                    .FromMonster(this)
                    .WithHitFx("vfx/vfx_attack_slash")
                    .Execute(null);
            }, new MultiAttackIntent(2, MultiHits)
        );

        bash.FollowUpState = twinStrike;
        twinStrike.FollowUpState = conflagration;
        conflagration.FollowUpState = bash;
        
        return new MonsterMoveStateMachine([bash, twinStrike, conflagration], bash);
    }
}