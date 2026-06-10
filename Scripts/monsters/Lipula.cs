using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using wylder.Scripts.cards;
using wylder.Scripts.powers;

namespace wylder.Scripts.monsters;

public class Lipula : CustomMonsterModel
{
	// 根据进阶提高最小血量，进阶8及以上为120，否则为100
	public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 1000, 800);

	// 根据进阶提高最大血量，进阶8及以上为140，否则为120
	public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 1000, 800);

	public bool IsChangeState = false;
	
	private int _originalHp;

	private bool _isPortalOpen;
	
	public override LocString Title
	{
		get
		{
			if (!IsPortalOpen)
			{
				return MonsterModel.L10NMonsterLookup("WYLDER-LIPULA.name");
			}
			return MonsterModel.L10NMonsterLookup("WYLDER-LIPULA.name2");
		}
	}
	
	private int OriginalHp
	{
		get
		{
			return _originalHp;
		}
		set
		{
			AssertMutable();
			_originalHp = value;
		}
	}

	private bool IsPortalOpen
	{
		get
		{
			return _isPortalOpen;
		}
		set
		{
			AssertMutable();
			_isPortalOpen = value;
		}
	}
	
	//二阶段图片
	private static String _picture2 = "res://wylder/scenes/Lipula/lipula_2.png";
	
	// 意图1的数值，伤害和格挡，根据进阶提高伤害
	private int BasicDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 7, 5);
	private int BasicBlock => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 32, 24);
	private int BasicMad => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 3, 2);
	// 意图2的数值，重击伤害，根据进阶提高伤害
	private int HeavyDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 30, 27);

	private readonly Queue<string> _freeNodes = new(
		new[]
		{
			"first",
			"second",
			"third",
			"forth"
		});

	private readonly Dictionary<string, MonsterModel> _nodeMonsters = new();
	private readonly Dictionary<string, string> _nodeTargets = new();
	// 怪物场景，如果你的场景没有挂载脚本，参考这个
	public override NCreatureVisuals? CreateCustomVisuals() => NodeFactory<NCreatureVisuals>.CreateFromScene("res://wylder/scenes/Lipula/lipula.tscn");

	// 如果你挂载了自己的自定义脚本，使用这个
	//public override string? CustomVisualPath => "res://wylder/scenes/Lipula/lipula.tscn";


	// 战斗开始时，在这里给自己上buff之类
	public override async Task AfterAddedToRoom()
	{
		await base.AfterAddedToRoom();
		OriginalHp = base.Creature.MaxHp;
		await CreatureCmd.SetMaxAndCurrentHp(base.Creature, 999999999m);
		base.Creature.ShowsInfiniteHp = true;
	}
	

	protected override MonsterMoveStateMachine GenerateMoveStateMachine()
	{
		var open = new MoveState(
			"DRAMATIC_OPEN", // 状态ID
			DramaticOpenMove, // 执行函数，或者直接用lambda也可
			// 以下是可变参数，可以填写任意数量的意图，全部展示
			new DebuffIntent(),
			new SummonIntent()
		);
		
		// 意图1：造成伤害，获得格挡
		var basicAttack = new MoveState(
			"BASIC_ATTACK", // 状态ID
			BasicAttackMove, // 执行函数，或者直接用lambda也可
			// 以下是可变参数，可以填写任意数量的意图，全部展示
			new MultiAttackIntent(BasicDamage,3),
			new StatusIntent(BasicMad)
		);

		// 意图2：重击
		var heavyAttack = new MoveState(
			"HEAVY_ATTACK",
			async targets =>
			{
				await DamageCmd // 意图2实际执行效果，这里直接用lambda
					.Attack(HeavyDamage)
					.FromMonster(this)
					.WithAttackerFx(null, AttackSfx)
					.WithHitFx("vfx/vfx_attack_blunt")
					.Execute(null);
				await CreatureCmd.TriggerAnim(base.Creature, "Cast", 0.5f);
				SfxCmd.Play(AttackSfx);
				await CardPileCmd.AddToCombatAndPreview<Mad>(targets, PileType.Discard, 1, addedByPlayer: false);
			}, new SingleAttackIntent(HeavyDamage),
			new StatusIntent(1)
		);

		var basicStand = new MoveState(
			"BASIC_STAND", async targets =>
			{
				await CreatureCmd.GainBlock(Creature, BasicBlock, ValueProp.Move, null);
				await PowerCmd.Apply<StrengthPower>(Creature, 2, Creature, null);
				if (IsChangeState)
				{
					await PowerCmd.Apply<IntangiblePower>(base.Creature, 2, Creature, null);
					await PowerCmd.Apply<ThornsPower>(Creature, 30, Creature, null);
				}
			}, new DefendIntent(), new BuffIntent()
			);

		var changeState = new MoveState("CHANGE_STATE", ChangeStateMove, new SingleAttackIntent(45), new BuffIntent());

		var callGuilty = new MoveState("CALL_GUILTY", CallGuilty, new SummonIntent(), new DebuffIntent());
		
		ConditionalBranchState conditionalBranchState = new ConditionalBranchState("lipulaBranch");
		conditionalBranchState.AddState(callGuilty, () => IsChangeState);
		conditionalBranchState.AddState(basicAttack, () => !IsChangeState);
		
		_changeState = changeState;
		// 或者你也可以创建RandomBranchState（随机意图分支）和ConditionalBranchState（条件意图分支）来实现更复杂的状态转换逻辑

		// 设置状态转换，意图1后接意图2，意图2后接意图1
		open.FollowUpState = basicAttack;
		basicAttack.FollowUpState = heavyAttack;
		heavyAttack.FollowUpState = basicStand;
		basicStand.FollowUpState = conditionalBranchState;
		changeState.FollowUpState = callGuilty;
		callGuilty.FollowUpState = basicAttack;
		
		// 添加2个意图，并且初始意图设成 basicAttack
		return new MonsterMoveStateMachine([open, basicAttack, heavyAttack, basicStand, changeState, callGuilty, conditionalBranchState], open);
	}
	
	private async Task DramaticOpenMove(IReadOnlyList<Creature> targets)
	{
		foreach (Creature creature in targets)
		{
			Log.Warn("note " + _nodeMonsters.Count);
			if (!creature.IsPlayer || creature.Player is null)
			{
				continue;
			}
		
			if (creature.Player.Character is Ironclad)
			{
				MonsterModel testMonster = ModelDb.Monster<NightIronclad>();
				if (_freeNodes.Count == 0)
					continue;

				string node = _freeNodes.Dequeue();

				_nodeMonsters[node] = testMonster;
				_nodeTargets[node] = creature.Player.Creature.Name;
			} else if (creature.Player.Character is Silent)
			{
				MonsterModel testMonster = ModelDb.Monster<NightSilent>();
				if (_freeNodes.Count == 0)
					continue;

				string node = _freeNodes.Dequeue();

				_nodeMonsters[node] = testMonster;
				_nodeTargets[node] = creature.Player.Creature.Name;
			}
			else
			{
				MonsterModel testMonster = ModelDb.Monster<NightIronclad>();
				if (_freeNodes.Count == 0)
					continue;

				string node = _freeNodes.Dequeue();

				_nodeMonsters[node] = testMonster;
				_nodeTargets[node] = creature.Player.Creature.Name;
			}
		}
		TalkCmd.Play(MonsterModel.L10NMonsterLookup("WYLDER-LIPULA.moves.DRAMATIC_OPEN.speakLine1"), base.Creature, VfxColor.Gold);
		await Cmd.CustomScaledWait(1.5f, 1.7f);
		List<Task> chooseList = new List<Task>();
		foreach (Creature target in targets)
		{
			chooseList.Add(ChooseCurse(target));
		}
		await Task.WhenAll(chooseList);
		TalkCmd.Play(MonsterModel.L10NMonsterLookup("WYLDER-LIPULA.moves.DRAMATIC_OPEN.speakLine2"), base.Creature, VfxColor.Gold);
		await Cmd.CustomScaledWait(1.5f, 1.7f);
		IsPortalOpen = true;
		await CreatureCmd.SetMaxAndCurrentHp(base.Creature, OriginalHp);
		List<PowerModel> list = base.Creature.Powers.ToList();
		foreach (PowerModel item in list)
		{
			await PowerCmd.Remove(item);
		}
		base.Creature.ShowsInfiniteHp = false;
		UpdateVisual(_picture2);
		await PowerCmd.Apply<HardenedShellPower>(base.Creature, 200m, base.Creature, null);
		await PowerCmd.Apply<LipulaChangeStatePower>(Creature, Creature.MaxHp * 0.75m, Creature, null);
		await Cmd.CustomScaledWait(0.2f, 0.6f);
		NRunMusicController.Instance?.UpdateMusicParameter("queen_progress", 1f);
	}

	public static MoveState _changeState;
	
	// 意图1执行实际效果
	private async Task BasicAttackMove(IReadOnlyList<Creature> targets)
	{
		await DamageCmd
			.Attack(BasicDamage)
			.WithHitCount(3)
			.FromMonster(this)
			// .WithAttackerAnim("Attack", 0.5f) // 如果有攻击动画，可以取消注释并替换成实际动画名称和延迟
			.WithAttackerFx(null, AttackSfx) // 攻击音效
			.WithHitFx("vfx/vfx_attack_blunt") // 攻击特效
			.Execute(null);
		await CreatureCmd.TriggerAnim(base.Creature, "Cast", 0.5f);
		SfxCmd.Play(AttackSfx);
		await CardPileCmd.AddToCombatAndPreview<Mad>(targets, PileType.Discard, BasicMad, addedByPlayer: false);
	}

	public async Task ChangeStateMove(IReadOnlyList<Creature> targets)
	{
		await DamageCmd
			.Attack(45)
			.FromMonster(this)
			// .WithAttackerAnim("Attack", 0.5f) // 如果有攻击动画，可以取消注释并替换成实际动画名称和延迟
			.WithAttackerFx(null, AttackSfx) // 攻击音效
			.WithHitFx("vfx/vfx_attack_blunt") // 攻击特效
			.Execute(null);
		await CreatureCmd.TriggerAnim(base.Creature, "Cast", 0.5f);
		SfxCmd.Play(AttackSfx);
		await PowerCmd.Apply<IntangiblePower>(base.Creature, 2, Creature, null);
		await PowerCmd.Apply<ThornsPower>(Creature, 30, Creature, null);
		await PowerCmd.Apply<HatredPower>(Creature, 1, Creature, null);
		PowerModel? power = base.Creature.GetPower<LipulaChangeStatePower>();
		if (power != null)
		{
			await PowerCmd.Remove(power);
		}

		IsChangeState = true;
	}

	public async Task CallGuilty(IReadOnlyList<Creature> targets)
	{
		TalkCmd.Play(MonsterModel.L10NMonsterLookup("WYLDER-LIPULA.moves.CALL_GUILTY.speakLine1"), base.Creature, VfxColor.Gold);
		await Cmd.CustomScaledWait(1.5f, 1.7f);
		foreach (String slotName in CombatState.Encounter.Slots)
		{
			if (!_nodeMonsters.ContainsKey(slotName))
			{
				continue;
			}
			Creature? creature = CombatState.Enemies
				.FirstOrDefault(c => c.SlotName == slotName);
			if (creature == null)
			{
				MonsterModel testMonster = _nodeMonsters[slotName];
				Creature newMonster = await CreatureCmd.Add(testMonster.ToMutable(), base.CombatState, CombatSide.Enemy, slotName);
				await PowerCmd.Apply<MinionPower>(newMonster, 1m, base.Creature, null);
				Hatred2Power? hatred2Power = await PowerCmd.Apply<Hatred2Power>(newMonster, 1m, base.Creature, null);
				if (hatred2Power != null) 
				{
					hatred2Power.TargetPlayer = _nodeTargets[slotName];
				}
			}
			else
			{
				await CreatureCmd.Heal(creature, 99999);
				await PowerCmd.Apply<StrengthPower>(creature, 3, Creature,  null);
			}
		}
		//MonsterModel testMonster = ModelDb.Monster<NightIronclad>();
		//await PowerCmd.Apply<MinionPower>(await CreatureCmd.Add(testMonster.ToMutable(), base.CombatState, CombatSide.Enemy, base.CombatState.Encounter.GetNextSlot(base.CombatState)), 1m, base.Creature, null);
		PowerModel? power = base.Creature.GetPower<ThornsPower>();
		if (power != null)
		{
			await PowerCmd.Remove(power);
		}
	}
	
	private void UpdateVisual(string path)
	{
		NCreature nCreature = NCombatRoom.Instance?.GetCreatureNode(base.Creature);
		if (nCreature != null)
		{
			((Sprite2D)nCreature.Visuals.GetCurrentBody()).Texture = PreloadManager.Cache.GetTexture2D(path);
			Vector2 scale = nCreature.Visuals.GetCurrentBody().Scale;
			Tween tween = nCreature.CreateTween();
			tween.TweenProperty(nCreature.Visuals.GetCurrentBody(), "scale", scale, 1.2000000476837158).From(scale * 0.5f).SetEase(Tween.EaseType.Out)
				.SetTrans(Tween.TransitionType.Sine);
			tween.Parallel().TweenProperty(nCreature.Visuals.GetCurrentBody(), "modulate", Colors.White, 0.5).From(Colors.Black);
		}
	}
	
	private async Task ChooseCurse(Creature target)
	{
		if (target.IsDead || target.Player == null)
		{
			return;
		}
		List<CardModel> cards = CardFactory.GetDistinctForCombat(target.Player, generateCards(), 3, target.Player.RunState.Rng.CombatCardGeneration).ToList();
		CardModel cardModel = await CardSelectCmd.FromChooseACardScreen(new BlockingPlayerChoiceContext(), cards, target.Player);
		if (cardModel != null)
		{
			await ((KnowledgeDemon.IChoosable)cardModel).OnChosen();
		}
	}
	
	private IEnumerable<CardModel> generateCards()
	{
		return (IEnumerable<CardModel>)new CardModel[]
		{
			ModelDb.Card<LipulaChoose7>(),
			ModelDb.Card<LipulaChoose6>(),
			ModelDb.Card<LipulaChoose5>(),
			ModelDb.Card<LipulaChoose4>(),
			ModelDb.Card<LipulaChoose3>(),
			ModelDb.Card<LipulaChoose2>(),
			ModelDb.Card<LipulaChoose1>(),
		};
	}
}
