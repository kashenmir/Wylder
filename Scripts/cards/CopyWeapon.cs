using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using wylder.Scripts.pools;

namespace wylder.Scripts.cards;

[Pool(typeof(WylderCardPool))]
public class CopyWeapon : TestCardModel
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
    // 卡牌的基础属性（例如这里是12点伤害）
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];

    public CopyWeapon() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    // 打出时的效果逻辑
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        CardModel selection = (await CardSelectCmd.FromHand(prefs: new CardSelectorPrefs(base.SelectionScreenPrompt, 1), context: choiceContext, player: base.Owner, filter: delegate(CardModel c)
        {
            CardType type = c.Type;
            return (type == CardType.Attack || type == CardType.Power) ? true : false;
        }, source: this)).FirstOrDefault();
        if (selection != null)
        {
            for (int i = 0; i < base.DynamicVars.Cards.IntValue; i++)
            {
                CardModel card = selection.CreateClone();
                await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, addedByPlayer: true);
            }
        }
    }

    // 升级后的效果逻辑
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}