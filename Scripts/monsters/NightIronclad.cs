using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Combat;
using DemonFormPower = wylder.Scripts.powers.DemonFormPower;

namespace wylder.Scripts.monsters;

public class NightIronclad : CustomMonsterModel
{
    // 根据进阶提高最小血量，进阶8及以上为120，否则为100
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 176, 160);

    // 根据进阶提高最大血量，进阶8及以上为140，否则为120
    public override int MaxInitialHp => MinInitialHp;

    // 怪物场景，如果你的场景没有挂载脚本，参考这个
    public override NCreatureVisuals? CreateCustomVisuals(){
        NCreatureVisuals? creatureVisuals = NodeFactory<NCreatureVisuals>.CreateFromScene("res://scenes/creature_visuals/ironclad.tscn");
        creatureVisuals.Modulate =  Color.FromHtml("59608c");
        creatureVisuals.Visible = false;
        return creatureVisuals;
    }
    private int HeavyDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 30, 20);
    
    private int DoubleDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 6, 5);

    private int MultiHits => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 5, 4);
    
    // 战斗开始时，在这里给自己上buff之类
    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
        await PowerCmd.Apply<IntangiblePower>(Creature, 2, Creature, null);
        await PowerCmd.Apply<RegenPower>(Creature, 6, Creature, null);
        float num = CombatState.RunState.Rng.MonsterAi.NextFloat(2);
        Log.Warn("rng num:"+num);
        if (num <= 1.0f)
        {
            await PowerCmd.Apply<DemonFormPower>(Creature, 5, Creature, null);
        }
        else
        {
            await PowerCmd.Apply<powers.RupturePower>(Creature, 1, Creature, null);
        }
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
                    .WithHitFx("vfx/vfx_attack_blunt", null, "blunt_attack.mp3")
                    .Execute(null);
                await PowerCmd.Apply<VulnerablePower>(targets, 4m, base.Creature, null);
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
                    .WithHitFx("vfx/vfx_attack_blunt", null, "heavy_attack.mp3")
                    .Execute(null);
            }, new MultiAttackIntent(2, MultiHits)
        );

        bash.FollowUpState = twinStrike;
        twinStrike.FollowUpState = conflagration;
        conflagration.FollowUpState = bash;
        
        return new MonsterMoveStateMachine([bash, twinStrike, conflagration], bash);
    }
}