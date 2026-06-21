using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using wylder.Scripts.dynamicVars;
using wylder.Scripts.pools;
using wylder.Scripts.powers;
using wylder.Scripts.relics;

namespace wylder.Scripts.cards;

[Pool(typeof(WylderCardPool))]
public class ClawShot : TestCardModel
{
    // 基础耗能
    private const int energyCost = 0;
    // 卡牌类型
    private const CardType type = CardType.Attack;
    // 卡牌稀有度
    private const CardRarity rarity = CardRarity.Basic;
    // 目标类型（AnyEnemy表示任意敌人）
    private const TargetType targetType = TargetType.AnyEnemy;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;
    public override bool CanBeGeneratedInCombat => false;
    //
    private const int maxPower = 12;
    
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

    // 卡牌的基础属性（例如这里是20点伤害）
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(1, ValueProp.Move), new Charges(maxPower), new IntVar("count", maxPower), new IntVar("blood", 3), new Chase(3)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<BloodBase>()];
    
    public ClawShot() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
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
            (DeckVersion as ClawShot)?.UpdateCharge();
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
    
    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        if (dealer == Owner.Creature && props.IsPoweredAttack() && cardSource == this)
        {
            await PowerCmd.Apply<BloodBase>(choiceContext, target, DynamicVars["blood"].IntValue, Owner.Creature, this);
        }
    }
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
		await DamageCmd.Attack(DynamicVars.Damage.BaseValue).WithHitCount(2).FromCard(this)
			.Targeting(cardPlay.Target)
			.WithHitFx("vfx/vfx_attack_slash")
			.Execute(choiceContext);
        await PowerCmd.Apply<BasicChasePower>(choiceContext, Owner.Creature, DynamicVars["Chase"].IntValue, Owner.Creature, this);
        ClearCharge();
        (DeckVersion as ClawShot)?.ClearCharge();
        await CardCmd.Exhaust(choiceContext,this, false, false);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
        DynamicVars["blood"].UpgradeValueBy(1m);
        DynamicVars["Chase"].UpgradeValueBy(2m);
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
}