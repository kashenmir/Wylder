using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.ValueProps;
using wylder.Scripts.powers;
using TimeDevourPower = wylder.Scripts.powers.TimeDevourPower;

namespace wylder.Scripts.monsters;

public class NightWatcher : CustomMonsterModel
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 200, 180);

    public override int MaxInitialHp => MinInitialHp;

    public override NCreatureVisuals? CreateCustomVisuals()
    {
        NCreatureVisuals? creatureVisuals = NodeFactory<NCreatureVisuals>.CreateFromScene("res://wylder/scenes/Watcher/night_watcher.tscn");
        creatureVisuals.Modulate = Color.FromHtml("3a2a5c");
        creatureVisuals.Visible = false;
        return creatureVisuals;
    }

    private int RagnarokDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 6, 5);

    private int RagnarokHits => 6;

    private int HeavyBlowDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 45, 36);

    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
        await PowerCmd.Apply<IntangiblePower>(new ThrowingPlayerChoiceContext(), Creature, 2, Creature, null);
        await PowerCmd.Apply<MentalFortressPower>(new ThrowingPlayerChoiceContext(), Creature, 5, Creature, null);
        await PowerCmd.Apply<TimeDevourPower>(new ThrowingPlayerChoiceContext(), Creature, 1, Creature, null);
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var ragnarok = new MoveState(
            "RAGNAROK",
            async targets =>
            {
                await DamageCmd
                    .Attack(RagnarokDamage)
                    .WithHitCount(RagnarokHits)
                    .FromMonster(this)
                    .WithHitFx("vfx/vfx_attack_lightning", "event:/sfx/characters/defect/defect_lightning_evoke", "heavy_attack.mp3")
                    .Execute(null);
            }, new MultiAttackIntent(RagnarokDamage, RagnarokHits)
        );

        var heavyBlow = new MoveState(
            "HEAVY_BLOW",
            async targets =>
            {
                AttackCommand attackCommand = await DamageCmd
                    .Attack(HeavyBlowDamage)
                    .FromMonster(this)
                    .WithHitFx("vfx/vfx_heavy_blunt", null, "heavy_attack.mp3")
                    .WithHitVfxSpawnedAtBase()
                    .Execute(null);
                await CreatureCmd.GainBlock(Creature, (int)(attackCommand.Results.SelectMany((List<DamageResult> r) => r).Sum((DamageResult r) => r.UnblockedDamage)), ValueProp.Move, null);
            }, new SingleAttackIntent(HeavyBlowDamage), new DefendIntent()
        );

        ragnarok.FollowUpState = heavyBlow;
        heavyBlow.FollowUpState = ragnarok;

        return new MonsterMoveStateMachine([ragnarok, heavyBlow], ragnarok);
    }
}
