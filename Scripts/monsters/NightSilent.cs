using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.ValueProps;
using SerpentFormPower = wylder.Scripts.powers.SerpentFormPower;

namespace wylder.Scripts.monsters;

public class NightSilent : CustomMonsterModel
{
       // 根据进阶提高最小血量，进阶8及以上为120，否则为100
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 130, 120);

    // 根据进阶提高最大血量，进阶8及以上为140，否则为120
    public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 150, 130);

    // 怪物场景，如果你的场景没有挂载脚本，参考这个
    public override NCreatureVisuals? CreateCustomVisuals() => NodeFactory<NCreatureVisuals>.CreateFromScene("res://wylder/scenes/silent/silent.tscn");

    private int HeavyDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 4, 3);
    
    private int DoubleDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 13, 10);

    private int MultiHits => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 5, 4);
    
    // 战斗开始时，在这里给自己上buff之类
    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
        await PowerCmd.Apply<IntangiblePower>(Creature, 2, Creature, null);
        await PowerCmd.Apply<SerpentFormPower>(Creature, 2, Creature, null);
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
                await PowerCmd.Apply<WeakPower>(targets, 2m, base.Creature, null);
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
                await CreatureCmd.GainBlock(Creature, 20, ValueProp.Move, null);
            }, new SingleAttackIntent(DoubleDamage), new DefendIntent()
        );
        
        var levelUp = new MoveState(
            "LEVEL_UP",
            async targets =>
            {
                await CreatureCmd.TriggerAnim(base.Creature, "Cast", 0.5f);
                await PowerCmd.Apply<SerpentFormPower>(Creature, 2, base.Creature, null);
            }, new BuffIntent()
        );

        dash.FollowUpState = neutralize;
        neutralize.FollowUpState = levelUp;
        levelUp.FollowUpState = dash;
        
        return new MonsterMoveStateMachine([neutralize, dash, levelUp], dash);
    }
}