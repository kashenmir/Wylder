using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using wylder.Scripts.pools;

namespace wylder.Scripts.cards;

[Pool(typeof(WylderCardPool))]
public class Shopping : TestCardModel
{
    // 基础耗能
    private const int energyCost = 1;
    // 卡牌类型
    private const CardType type = CardType.Skill;
    // 卡牌稀有度
    private const CardRarity rarity = CardRarity.Uncommon;
    // 目标类型（AnyEnemy表示任意敌人）
    private const TargetType targetType = TargetType.Self;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;
    
    protected override bool IsPlayable => (Owner.Gold >= 30);
    
    public Shopping() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<StarSharp>(), HoverTipFactory.FromCard<TurtleNeck>()];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        List<CardModel> items = CardFactory.GetDistinctForCombat(base.Owner, generateNightCards(), 1, base.Owner.RunState.Rng.CombatCardGeneration).ToList();
        List<CardModel> gudings = CardFactory.GetDistinctForCombat(base.Owner, new CardModel[]
        {
            ModelDb.Card<TurtleNeck>(),
            ModelDb.Card<StarSharp>(),
        }, 2, base.Owner.RunState.Rng.CombatCardGeneration).ToList();
        items.AddRange(gudings);
        for (int i=0; i<3; i++)
        {
            CardModel cardModel;
            List<CardModel> cards = CardFactory.GetDistinctForCombat(base.Owner,
                    base.Owner.Character.CardPool.GetUnlockedCards(base.Owner.UnlockState,
                        base.Owner.RunState.CardMultiplayerConstraint), 2, base.Owner.RunState.Rng.CombatCardGeneration)
                .ToList();
            cards.Add(items[i]);
            if (IsUpgraded)
            {
                foreach (CardModel item in cards)
                {
                    CardCmd.Upgrade(item);
                }
            }
            cardModel = await CardSelectCmd.FromChooseACardScreen(choiceContext, cards, base.Owner, canSkip: true);
            if (cardModel != null)
            {
                cardModel.SetToFreeThisTurn();
                int money = 15;
                if (cardModel.Rarity == CardRarity.Uncommon)
                {
                    money = 20;
                } else if (cardModel.Rarity == CardRarity.Rare)
                {
                    money = 30;
                }
                else if (cardModel is StarSharp)
                {
                    money = 20;
                }
                await PlayerCmd.LoseGold(money, Owner);
                await CardPileCmd.AddGeneratedCardToCombat(cardModel, PileType.Hand, null);
                return;
            }
        }
    }

    
    protected override void OnUpgrade()
    {
    }
    
    private IEnumerable<CardModel> generateNightCards()
    {
        return (IEnumerable<CardModel>)new CardModel[]
        {
            ModelDb.Card<FirePot>(),
            ModelDb.Card<BloodGrease>(),
            ModelDb.Card<Flesh>(),
            ModelDb.Card<Crab>(),
            ModelDb.Card<FirePot>(),
            ModelDb.Card<Flesh>(),
            ModelDb.Card<Crab>(),
            ModelDb.Card<FireGrease>(),
            ModelDb.Card<FreezingGrease>(),
            ModelDb.Card<Flesh>(),
            ModelDb.Card<Crab>(),
            ModelDb.Card<PoisonGrease>(),
            ModelDb.Card<RotGrease>(),
            ModelDb.Card<SleepGrease>(),
            ModelDb.Card<WarmRock>()
        };
    }
}