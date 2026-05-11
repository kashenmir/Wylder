using wylder.Scripts.pools;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using wylder.Scripts.dynamicVars;
using wylder.Scripts.powers;
using wylder.Scripts.relics;

namespace wylder.Scripts.cards;

[Pool(typeof(WylderCardPool))]
public class OnslaughtStake : TestCardModel
{
    // 基础耗能
    private const int energyCost = 3;
    // 卡牌类型
    private const CardType type = CardType.Attack;
    // 卡牌稀有度
    private const CardRarity rarity = CardRarity.Rare;
    // 目标类型（AnyEnemy表示任意敌人）
    private const TargetType targetType = TargetType.AllEnemies;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;
    
    //
    private const int maxPower = 90;
    
    protected override bool IsPlayable => ChargeCount >= maxPower;

    private int _chargeCount = maxPower;
    
    [SavedProperty]
    public int ChargeCount
    {
        get
        {
            return _chargeCount;
        }
        set
        {
            AssertMutable();
            _chargeCount = value;
            DynamicVars["count"].UpgradeValueBy(value-DynamicVars["count"].IntValue);
        }
    }
    
    public void UpdateCharge()
    {
        Lune? lune = Owner.Relics.OfType<Lune>().FirstOrDefault();
        if (lune != null)
        {
            ChargeCount += 3;
        } else
        {
            ChargeCount += 2;
        }
    }

    public void ClearCharge()
    {
        ChargeCount = 0;
    }

    // 卡牌的基础属性（例如这里是20点伤害）
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(30, ValueProp.Move), new Charges(maxPower), new IntVar("count", maxPower)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [StunIntent.GetStaticHoverTip()];
    
    public OnslaughtStake() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }
    
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner)
        {
            return;
        }

        if (cardPlay.Card == this)
        {
            return;
        }
        
        CardPile? pile = Pile;
        if (pile != null && pile.Type == PileType.Exhaust)
        {
            UpdateCharge();
            (DeckVersion as OnslaughtStake)?.UpdateCharge();
            if (DynamicVars["count"].IntValue >= DynamicVars["Charges"].IntValue)
            {
                await CardPileCmd.Add(this, PileType.Hand);
            }
        }
        return;
    }
    
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, CombatState combatState)
    {
        if (player == Owner && combatState.RoundNumber <= 1 && ChargeCount<maxPower)
        {
            await CardCmd.Exhaust(choiceContext,this, false, false);
        }
    }
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(CombatState)
            .WithHitFx("vfx/vfx_attack_blunt", null, "heavy_attack.mp3")
            .Execute(choiceContext);
        List<Task> stunTasks = new List<Task>();
        foreach (Creature hittableEnemy in CombatState.HittableEnemies)
        {
            Frost? frost = hittableEnemy.GetPower<Frost>();
            if (frost != null)
            {
                await PowerCmd.Remove(frost);
            }
            NCreature nCreature = NCombatRoom.Instance?.GetCreatureNode(hittableEnemy);
            if (nCreature != null)
            {
                stunTasks.Add(CreatureCmd.Stun(hittableEnemy));
            }
        }
        await Task.WhenAll(stunTasks);
        ClearCharge();
        (DeckVersion as OnslaughtStake)?.ClearCharge();
        await CardCmd.Exhaust(choiceContext,this, false, false);
    }
    
    public static async Task Stun(Creature creature, String? nextMoveId)
    {
        if (creature.Monster == null)
        {
            throw new InvalidOperationException("Can't stun a player.");
        }
        if (creature.CombatState != null && !creature.IsDead)
        {
            if (nextMoveId == null)
            {
                List<MonsterState> stateLog = creature.Monster.MoveStateMachine.StateLog;
                nextMoveId = stateLog.Last().Id;
            }
            creature.Monster.SetMoveImmediate(new MoveState("STUNNED", Wrapper, new AbstractIntent[1]
            {
                (AbstractIntent) new StunIntent()
            })
            {
                FollowUpStateId = nextMoveId,
                MustPerformOnceBeforeTransitioning = true
            }, true);
        }
        async Task Wrapper(IReadOnlyList<Creature> c)
        {
            // NStunnedVfx? vfx = NStunnedVfx.Create(Owner.Monster.Creature);
            // if (vfx != null)
            // {
            //     Callable.From(delegate
            //     {
            //         NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(vfx);
            //     }).CallDeferred();
            // }
            await Task.CompletedTask;
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(10m);
    }
}