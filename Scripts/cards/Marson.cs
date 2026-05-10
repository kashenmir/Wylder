using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using wylder.Scripts.pools;

namespace wylder.Scripts.cards;

[Pool(typeof(WylderCardPool))]
public class Marson : TestCardModel
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
    
    public override bool GainsBlock => true;
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(1, ValueProp.Move)];

    public Marson() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    // 打出时的效果逻辑
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int blockValue = 0; 
        foreach (CardModel item in CardPile.GetCards(base.Owner, PileType.Hand).ToList())
        {
            if (item.Type != CardType.Skill && item.Type != CardType.Power)
            {
                await CardCmd.Discard(choiceContext, item);
                blockValue++;
            }
        }
        CardModel cardModel;
        do
        {
            cardModel = await CardPileCmd.Draw(choiceContext, base.Owner);
            if (cardModel.Type != CardType.Skill && cardModel.Type != CardType.Power)
            {
                await CardCmd.Discard(choiceContext, cardModel);
                blockValue++;
            }
        } while (cardModel != null && CardPile.GetCards(base.Owner, PileType.Hand).Count() < 10 && countCards(Owner)>0 );
        
        await CreatureCmd.GainBlock(base.Owner.Creature, new BlockVar(blockValue, ValueProp.Move), cardPlay);
    }

    // 升级后的效果逻辑
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
    
    private static int countCards(Player owner)
    {
        return owner.PlayerCombatState.AllCards.Where((CardModel c) => (c.Type == CardType.Skill || c.Type == CardType.Power) && (c.Pile.Type == PileType.Draw || c.Pile.Type == PileType.Discard)).Count();
    }
}