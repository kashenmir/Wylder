using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using wylder.Scripts.pools;

namespace wylder.Scripts.cards;

// 加入哪个卡池
[Pool(typeof(WylderCardPool))]
public class SmallPouch : TestCardModel
{
    // 基础耗能
    private const int energyCost = 0;
    // 卡牌类型
    private const CardType type = CardType.Skill;
    // 卡牌稀有度
    private const CardRarity rarity = CardRarity.Common;
    // 目标类型（AnyEnemy表示任意敌人）
    private const TargetType targetType = TargetType.Self;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    public SmallPouch() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    // 打出时的效果逻辑
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        CardModel cardModel = CardFactory.GetDistinctForCombat(base.Owner, generateNightCards(), 1, base.Owner.RunState.Rng.CombatCardGeneration).FirstOrDefault();
        if (cardModel != null)
        {
            if (base.IsUpgraded)
            {
                CardCmd.Upgrade(cardModel);
            }
            await CardPileCmd.AddGeneratedCardToCombat(cardModel, PileType.Hand, null);
        }
    }

    // 升级后的效果逻辑
    protected override void OnUpgrade()
    {
    }
    
    private IEnumerable<CardModel> generateNightCards()
    {
        return (IEnumerable<CardModel>)new CardModel[]
        {
            ModelDb.Card<FireGrease>(),
            ModelDb.Card<FreezingGrease>(),
            ModelDb.Card<PoisonGrease>(),
            ModelDb.Card<RotGrease>(),
            ModelDb.Card<SleepGrease>(),
            ModelDb.Card<BloodGrease>(),
            ModelDb.Card<Flesh>(),
            ModelDb.Card<Crab>(),
            ModelDb.Card<TurtleNeck>(),
            ModelDb.Card<FirePot>(),
            ModelDb.Card<Boluses>(),
            ModelDb.Card<Flesh>(),
            ModelDb.Card<Crab>(),
            ModelDb.Card<TurtleNeck>(),
            ModelDb.Card<FirePot>(),
            ModelDb.Card<Boluses>(),
            ModelDb.Card<StarSharp>(),
        };
    }
}