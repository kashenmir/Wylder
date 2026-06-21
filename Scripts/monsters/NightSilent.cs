using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using SerpentFormPower = wylder.Scripts.powers.SerpentFormPower;

namespace wylder.Scripts.monsters;

public class NightSilent : CustomMonsterModel
{
       // 根据进阶提高最小血量，进阶8及以上为120，否则为100
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 154, 140);

    // 根据进阶提高最大血量，进阶8及以上为140，否则为120
    public override int MaxInitialHp => MinInitialHp;

    // 怪物场景，如果你的场景没有挂载脚本，参考这个
    public override NCreatureVisuals? CreateCustomVisuals(){
        NCreatureVisuals? creatureVisuals = NodeFactory<NCreatureVisuals>.CreateFromScene("res://scenes/creature_visuals/silent.tscn");
        creatureVisuals.Modulate =  Color.FromHtml("59608c");
        creatureVisuals.Visible = false;
        return creatureVisuals;
    }
    private int HeavyDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 17, 13);
    
    private int DoubleDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 13, 10);

    private int SnakebiteValue => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 9, 7);
    
    // 战斗开始时，在这里给自己上buff之类
    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
        await PowerCmd.Apply<IntangiblePower>(new ThrowingPlayerChoiceContext(), Creature, 2, Creature, null);
        await PowerCmd.Apply<ThornsPower>(new ThrowingPlayerChoiceContext(), Creature, 3, Creature, null);
        float num = CombatState.RunState.Rng.MonsterAi.NextFloat(2);
        Log.Warn("rng num:"+num);
        if (num <= 1.0f)
        {
            await PowerCmd.Apply<SerpentFormPower>(new ThrowingPlayerChoiceContext(), Creature, 2, Creature, null);
        }
        else
        {
            await PowerCmd.Apply<powers.AfterimagePower>(new ThrowingPlayerChoiceContext(), Creature, 1, Creature, null);
        }
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var neutralize = new MoveState(
            "NEUTRALIZE",
            async targets =>
            {
                //await CreatureCmd.TriggerAnim(base.Creature, "Attack", 0.5f);
                await DamageCmd 
                    .Attack(HeavyDamage)
                    .FromMonster(this)
                    .WithHitFx("vfx/vfx_attack_blunt")
                    .Execute(null);
                await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), targets, 4m, base.Creature, null);
            }, new SingleAttackIntent(HeavyDamage), new DebuffIntent()
        );
        
        var dash = new MoveState(
            "DASH",
            async targets =>
            {
                //await CreatureCmd.TriggerAnim(base.Creature, "Attack", 0.5f);
                await DamageCmd 
                    .Attack(DoubleDamage)
                    .FromMonster(this)
                    .WithHitFx("vfx/vfx_attack_slash")
                    .Execute(null);
                await CreatureCmd.GainBlock(Creature, DoubleDamage, ValueProp.Move, null);
            }, new SingleAttackIntent(DoubleDamage), new DefendIntent()
        );
        
        var snakebite = new MoveState(
            "SNAKEBITE",
            async targets =>
            {
                await CreatureCmd.TriggerAnim(base.Creature, "Cast", 0.5f);
                foreach (Creature creature in targets)
                {
                    VfxCmd.PlayOnCreatureCenter(creature, "vfx/vfx_bite");
                }
                await PowerCmd.Apply<powers.PoisonPower>(new ThrowingPlayerChoiceContext(), targets, SnakebiteValue, base.Creature, null);
            }, new DebuffIntent()
        );

        dash.FollowUpState = snakebite;
        neutralize.FollowUpState = dash;
        snakebite.FollowUpState = neutralize;
        
        return new MonsterMoveStateMachine([neutralize, dash, snakebite], snakebite);
    }
}