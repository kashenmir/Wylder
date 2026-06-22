using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Combat;
using wylder.Scripts.cards;

namespace wylder.Scripts.monsters;

public class NightNecrobinder : CustomMonsterModel
{  
        // 根据进阶提高最小血量，进阶8及以上为120，否则为100
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 145, 132);

    // 根据进阶提高最大血量，进阶8及以上为140，否则为120
    public override int MaxInitialHp => MinInitialHp;

    // 怪物场景，如果你的场景没有挂载脚本，参考这个
    public override NCreatureVisuals? CreateCustomVisuals()
    {
        NCreatureVisuals? creatureVisuals = NodeFactory<NCreatureVisuals>.CreateFromScene("res://scenes/creature_visuals/necrobinder.tscn");
        creatureVisuals.Modulate =  Color.FromHtml("59608c");
        creatureVisuals.Visible = false;
        return creatureVisuals;
    }

    private int EnfeeblingValue => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 12, 8);
    
    private int DoubleDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 4, 3);

    private int BuryDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 63, 52);
    
    // 战斗开始时，在这里给自己上buff之类
    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
        await PowerCmd.Apply<IntangiblePower>(new ThrowingPlayerChoiceContext(), Creature, 3, Creature, null);
        float num = CombatState.RunState.Rng.MonsterAi.NextFloat(2);
        Log.Warn("rng num:"+num);
        if (num <= 1.0f)
        {
            await PowerCmd.Apply<powers.HauntPower>(new ThrowingPlayerChoiceContext(), Creature, 3, Creature, null);
        }
        else
        {
            await PowerCmd.Apply<powers.CallOfVoidPower>(new ThrowingPlayerChoiceContext(), Creature, 3, Creature, null);
        }
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var enfeebling = new MoveState(
            "ENFEEBLING",
            async targets =>
            {
                await CreatureCmd.TriggerAnim(base.Creature, "Cast", 0.5f);
                await PowerCmd.Apply<EnfeeblingTouchPower>(new ThrowingPlayerChoiceContext(), targets, EnfeeblingValue, base.Creature, null);
            }, new DebuffIntent()
        );
        
        var captureSpirit = new MoveState(
            "CAPTURE_SPIRIT",
            async targets =>
            {
                //await CreatureCmd.TriggerAnim(base.Creature, "Attack", 0.5f);
                await DamageCmd 
                    .Attack(DoubleDamage)
                    .FromMonster(this)
                    .WithHitFx("vfx/vfx_attack_slash")
                    .WithHitVfxSpawnedAtBase()
                    .Execute(null);
                await CardPileCmd.AddToCombatAndPreview<NightSoul>(targets, PileType.Draw, 4, null, CardPilePosition.Random);
            }, new SingleAttackIntent(DoubleDamage), new StatusIntent(4)
        );
        
        var bury = new MoveState(
            "BURY",
            async targets =>
            {
                await DamageCmd 
                    .Attack(BuryDamage)
                    .FromMonster(this)
                    .WithHitFx("vfx/vfx_attack_blunt", null, "blunt_attack.mp3")
                    .WithHitVfxSpawnedAtBase()
                    .Execute(null);
            }, new SingleAttackIntent(BuryDamage)
        );

        enfeebling.FollowUpState = captureSpirit;
        captureSpirit.FollowUpState = bury;
        bury.FollowUpState = enfeebling;
        
        return new MonsterMoveStateMachine([enfeebling, captureSpirit, bury], enfeebling);
    }
}