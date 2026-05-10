using wylder.Scripts.pools;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace wylder.Scripts.cards;

// 加入哪个卡池
[Pool(typeof(WylderCardPool))]
public class EochaidDancingBlade : AshWarModel
{
    // 基础耗能
    private const int energyCost = 0;
    protected override bool HasEnergyCostX => true;
    // 卡牌类型
    private const CardType type = CardType.Attack;
    // 卡牌稀有度
    private const CardRarity rarity = CardRarity.Rare;
    // 目标类型（AnyEnemy表示任意敌人）
    private const TargetType targetType = TargetType.AnyEnemy;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;
    
    private const string _increaseKey = "Increase";

    private const int _baseDamage = 5;

    private int _currentDamage = 5;

    private int _increasedDamage;

    [SavedProperty]
    public int CurrentDamage
    {
        get
        {
            return _currentDamage;
        }
        set
        {
            AssertMutable();
            _currentDamage = value;
            DynamicVars.Damage.BaseValue = _currentDamage;
        }
    }

    [SavedProperty]
    public int IncreasedDamage
    {
        get
        {
            return _increasedDamage;
        }
        set
        {
            AssertMutable();
            _increasedDamage = value;
        }
    }

    // 卡牌的基础属性（例如这里是3点伤害）
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(CurrentDamage, ValueProp.Move),new IntVar("Increase", 1m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Fatal)];
    
    public EochaidDancingBlade() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        bool shouldTriggerFatal = cardPlay.Target.Powers.All((PowerModel p) => p.ShouldOwnerDeathTriggerFatal());
        AttackCommand attackCommand = await DamageCmd.Attack(DynamicVars.Damage.BaseValue).WithHitCount(ResolveEnergyXValue()).FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        if (shouldTriggerFatal && attackCommand.Results.Any((DamageResult r) => r.WasTargetKilled))
        {
            int intValue = DynamicVars["Increase"].IntValue;
            BuffFromPlay(intValue);
            (DeckVersion as EochaidDancingBlade)?.BuffFromPlay(intValue);
        }
    }

    // 升级后的效果逻辑
    protected override void OnUpgrade()
    {
        DynamicVars["Increase"].UpgradeValueBy(1m);
    }
    
    protected override void AfterDowngraded()
    {
        UpdateDamage();
    }

    private void BuffFromPlay(int extraDamage)
    {
        IncreasedDamage += extraDamage;
        UpdateDamage();
    }

    private void UpdateDamage()
    {
        CurrentDamage = 5 + IncreasedDamage;
    }
}