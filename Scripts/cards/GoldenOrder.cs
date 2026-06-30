using System.Runtime.CompilerServices;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using wylder.Scripts.dynamicVars;
using wylder.Scripts.pools;
using wylder.Scripts.powers;

namespace wylder.Scripts.cards;

[Pool(typeof(WylderCardPool))]
public class GoldenOrder : AshWarModel
{
    // 基础耗能
    private const int energyCost = 1;
    // 卡牌类型
    private const CardType type = CardType.Skill;
    // 卡牌稀有度
    private const CardRarity rarity = CardRarity.Rare;
    // 目标类型（AnyEnemy表示任意敌人）
    private const TargetType targetType = TargetType.Self;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;
    // 卡牌的基础属性（例如这里是12点伤害）
    protected override IEnumerable<DynamicVar> CanonicalVars => [new Chase(8),
        new CalculationBaseVar(0m),
        new ExtraDamageVar(12m),
        new CalculatedDamageVar(ValueProp.Unpowered).WithMultiplier(YourCalculationFunction)];

    public GoldenOrder() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    // 打出时的效果逻辑
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int countDamage = 0;
        foreach (CardModel item in GetStatusesAndCurse(base.Owner).ToList())
        {
            await CardCmd.Exhaust(choiceContext, item);
            countDamage += DynamicVars["Chase"].IntValue;
        }

        foreach (PowerModel power in GetPowers(base.Owner))
        {
            await PowerCmd.Remove(power);
            countDamage += DynamicVars["Chase"].IntValue;
        }
        await PowerCmd.Apply<BasicChasePower>(choiceContext, Owner.Creature, countDamage, Owner.Creature, null);
    }
    
    private static decimal YourCalculationFunction(CardModel card, Creature? target)
    {
        // 在这里写你的计算逻辑
        // 例如，假设你要计算拥有者身上的某种状态数量
        // return card.Owner.SomeStatuses.Count(); 
    
        // 请替换为你的实际逻辑
        return GetPowers(card.Owner).Count() + GetStatusesAndCurse(card.Owner).Count();
    }
    
    private static IEnumerable<CardModel> GetStatusesAndCurse(Player owner)
    {
        return owner.PlayerCombatState.AllCards.Where((CardModel c) => (c.Type == CardType.Status || c.Type == CardType.Curse) && c.Pile.Type != PileType.Exhaust);
    }

    private static IEnumerable<PowerModel> GetPowers(Player owner)
    {
        return owner.Creature.Powers.ToList();
    }

    // 升级后的效果逻辑
    protected override void OnUpgrade()
    {
        base.DynamicVars.ExtraDamage.UpgradeValueBy(4m);
    }
}