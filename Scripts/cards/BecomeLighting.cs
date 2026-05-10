using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using wylder.Scripts.dynamicVars;
using wylder.Scripts.pools;

namespace wylder.Scripts.cards;

[Pool(typeof(WylderCardPool))]
public class BecomeLighting : AshWarModel
{
    // 基础耗能
    private const int energyCost = 1;
    // 卡牌类型
    private const CardType type = CardType.Attack;
    // 卡牌稀有度
    private const CardRarity rarity = CardRarity.Rare;
    // 目标类型（AnyEnemy表示任意敌人）
    private const TargetType targetType = TargetType.AnyEnemy;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;
    
    // 卡牌的基础属性（例如这里是12点伤害）
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4, ValueProp.Move), new CardsVar(5), new EnergyVar(1), new Tuci(1)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [EnergyHoverTip];
    
    public BecomeLighting() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }
    
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (cardSource != this)
        {
            return 1m;
        }

        if (target != null && target.Monster != null && target.Monster.IntendsToAttack)
        {
            return 1.5m;
        }
        return 1m;
    }
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        int attackCount = 1;
        int addDamage = 0;
        foreach (CardModel item in await CardSelectCmd.FromHand(prefs: new CardSelectorPrefs(base.SelectionScreenPrompt, 0, DynamicVars.Cards.IntValue), context: choiceContext, player: base.Owner, filter: null, source: this))
        {
            await CardCmd.Exhaust(choiceContext, item);
            if (item.Type == CardType.Attack)
            {
                addDamage += 6;
            } else if (item.Type == CardType.Skill)
            {
                attackCount++;
            } else
            {
                await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
            }
        }
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue+addDamage).WithHitCount(attackCount).FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(4);
    }
}