using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace wylder.Scripts.monsters;

public class NightDefect : CustomMonsterModel
{  
        // 根据进阶提高最小血量，进阶8及以上为120，否则为100
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 165, 150);

    // 根据进阶提高最大血量，进阶8及以上为140，否则为120
    public override int MaxInitialHp => MinInitialHp;

    // 怪物场景，如果你的场景没有挂载脚本，参考这个
    public override NCreatureVisuals? CreateCustomVisuals()
    {
        NCreatureVisuals? creatureVisuals = NodeFactory<NCreatureVisuals>.CreateFromScene("res://scenes/creature_visuals/defect.tscn");
        creatureVisuals.Modulate =  Color.FromHtml("59608c");
        creatureVisuals.Visible = false;
        return creatureVisuals;
    }

    private int GunkUpHits => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 4, 3);
    
    private int OverclockCount => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 3, 2);

    private int BoostBlock => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 24, 18);
    
    // 战斗开始时，在这里给自己上buff之类
    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
        await PowerCmd.Apply<IntangiblePower>(new ThrowingPlayerChoiceContext(), Creature, 2, Creature, null);
        await PowerCmd.Apply<ArtifactPower>(new ThrowingPlayerChoiceContext(), Creature, 3, Creature, null);
        float num = CombatState.RunState.Rng.MonsterAi.NextFloat(2);
        Log.Warn("rng num:"+num);
        if (num <= 1.0f)
        {
            int playerCount = CombatState.Players.Count;
            if (playerCount >= 3)
            {
                playerCount -= 1;
            }
            await PowerCmd.Apply<powers.CoolantPower>(new ThrowingPlayerChoiceContext(), Creature, playerCount, Creature, null);
        }
        else
        {
            await PowerCmd.Apply<powers.CreativeAiPower>(new ThrowingPlayerChoiceContext(), Creature, 2, Creature, null);
        }
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var gunkUp = new MoveState(
            "GUNK_UP",
            async targets =>
            {
                //await CreatureCmd.TriggerAnim(base.Creature, "Attack", 0.5f);
                await DamageCmd 
                    .Attack(5)
                    .WithHitCount(GunkUpHits)
                    .FromMonster(this)
                    .WithHitFx(null, null, "blunt_attack.mp3")
                    .WithHitVfxNode(NGoopyImpactVfx.Create)
                    .Execute(null);
                await CardPileCmd.AddToCombatAndPreview<Slimed>(targets, PileType.Discard, 2, null);
            }, new MultiAttackIntent(4, GunkUpHits), new StatusIntent(2)
        );
        
        var overclock = new MoveState(
            "OVERCLOCK",
            async targets =>
            {
                await CreatureCmd.TriggerAnim(base.Creature, "Cast", 0.5f);
                foreach (Creature creature in targets)
                {
                    NFireBurningVfx? child = NFireBurningVfx.Create(creature, 1f, goingRight: false);
                    if (child != null)
                    {
                        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(child);
                    }
                }
                await CardPileCmd.AddToCombatAndPreview<Burn>(targets, PileType.Draw, OverclockCount, null, CardPilePosition.Top);
            }, new StatusIntent(OverclockCount)
        );
        
        var boostAway = new MoveState(
            "BOOST_AWAY",
            async targets =>
            {
                await CreatureCmd.TriggerAnim(base.Creature, "Cast", 0.5f);
                await CreatureCmd.GainBlock(Creature, BoostBlock, ValueProp.Move, null);
                List<Task> statusTasks = new List<Task>();
                statusTasks.Add(CardPileCmd.AddToCombatAndPreview<Dazed>(targets, PileType.Discard, 2, null, CardPilePosition.Random));
                statusTasks.Add(CardPileCmd.AddToCombatAndPreview<Dazed>(targets, PileType.Draw, 2, null, CardPilePosition.Random));
                await Task.WhenAll(statusTasks);
            }, new DefendIntent(), new StatusIntent(4)
        );

        gunkUp.FollowUpState = overclock;
        overclock.FollowUpState = boostAway;
        boostAway.FollowUpState = gunkUp;
        
        return new MonsterMoveStateMachine([gunkUp, overclock, boostAway], gunkUp);
    }
}