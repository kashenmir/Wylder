using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.ValueProps;

namespace wylder.Scripts.monsters;

public class NightRegent : CustomMonsterModel
{  
        // 根据进阶提高最小血量，进阶8及以上为120，否则为100
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 165, 150);

    // 根据进阶提高最大血量，进阶8及以上为140，否则为120
    public override int MaxInitialHp => MinInitialHp;

    // 怪物场景，如果你的场景没有挂载脚本，参考这个
    public override NCreatureVisuals? CreateCustomVisuals()
    {
        NCreatureVisuals? creatureVisuals = NodeFactory<NCreatureVisuals>.CreateFromScene("res://scenes/creature_visuals/regent.tscn");
        creatureVisuals.Modulate =  Color.FromHtml("59608c");
        creatureVisuals.Visible = false;
        return creatureVisuals;
    }

    private int HeavyDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 18, 14);
    
    private int DoubleDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 26, 21);

    private int ReflectBlock => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 21, 17);
    
    // 战斗开始时，在这里给自己上buff之类
    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
        await PowerCmd.Apply<IntangiblePower>(Creature, 2, Creature, null);
        await PowerCmd.Apply<PlatingPower>(Creature, 10, Creature, null);
        float num = CombatState.RunState.Rng.MonsterAi.NextFloat(2);
        Log.Warn("rng num:"+num);
        if (num <= 1.0F)
        {
            await PowerCmd.Apply<powers.OrbitPower>(Creature, 1, Creature, null);
        }
        else
        {
            await PowerCmd.Apply<powers.ArsenalPower>(Creature, 1, Creature, null);
        }
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var fallingStar = new MoveState(
            "FALLING_STAR",
            async targets =>
            {
                //await CreatureCmd.TriggerAnim(base.Creature, "Attack", 0.5f);
                await DamageCmd 
                    .Attack(HeavyDamage)
                    .FromMonster(this)
                    .WithHitFx("vfx/vfx_starry_impact", "blunt_attack.mp3")
                    .Execute(null);
                await PowerCmd.Apply<WeakPower>(targets, 2m, base.Creature, null);
                await PowerCmd.Apply<VulnerablePower>(targets, 2m, base.Creature, null);
            }, new SingleAttackIntent(HeavyDamage), new DebuffIntent()
        );
        
        var crashLanding = new MoveState(
            "CRASH_LANDING",
            async targets =>
            {
                //await CreatureCmd.TriggerAnim(base.Creature, "Attack", 0.5f);
                await DamageCmd 
                    .Attack(DoubleDamage)
                    .FromMonster(this)
                    .WithHitFx("vfx/vfx_heavy_blunt", null, "heavy_attack.mp3")
                    .WithHitVfxSpawnedAtBase()
                    .Execute(null);
                await CardPileCmd.AddToCombatAndPreview<Debris>(targets, PileType.Hand, 5, addedByPlayer: false);
            }, new SingleAttackIntent(DoubleDamage), new StatusIntent(5)
        );
        
        var reflect = new MoveState(
            "REFLECT",
            async targets =>
            {
                await CreatureCmd.TriggerAnim(base.Creature, "Cast", 0.5f);
                await CreatureCmd.GainBlock(Creature, ReflectBlock, ValueProp.Move, null);
                await PowerCmd.Apply<ReflectPower>(Creature, 1, base.Creature, null);
            }, new DefendIntent(), new BuffIntent()
        );

        fallingStar.FollowUpState = crashLanding;
        crashLanding.FollowUpState = reflect;
        reflect.FollowUpState = fallingStar;
        
        return new MonsterMoveStateMachine([fallingStar, crashLanding, reflect], fallingStar);
    }
}