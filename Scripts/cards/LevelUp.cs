using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using wylder.Scripts.pools;
using wylder.Scripts.powers;

namespace wylder.Scripts.cards;

[Pool(typeof(WylderCardPool))]
public class LevelUp : TestCardModel
{
    // 基础耗能
    private const int energyCost = 1;
    // 卡牌类型
    private const CardType type = CardType.Power;
    // 卡牌稀有度
    private const CardRarity rarity = CardRarity.Uncommon;
    // 目标类型（AnyEnemy表示任意敌人）
    private const TargetType targetType = TargetType.Self;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("s", 2m), new IntVar("d", 3), new IntVar("t", 4)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>(), HoverTipFactory.FromPower<MetallicizePower>(), HoverTipFactory.FromPower<MallablePower>()];
    
    public LevelUp() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        CardModel cardModel1 = base.CombatState.CreateCard((ChooseStrength)ModelDb.Card<ChooseStrength>(), Owner);
        CardModel cardModel2 = base.CombatState.CreateCard((ChooseDefend)ModelDb.Card<ChooseDefend>(), Owner);
        CardModel cardModel3 = base.CombatState.CreateCard((ChooseTough)ModelDb.Card<ChooseTough>(), Owner);
        List<CardModel> cards = [cardModel1, cardModel2, cardModel3];
        if (IsUpgraded)
        {
            foreach (CardModel card in cards)
            {
                CardCmd.Upgrade(card);
            }
        }
        CardModel? cardModel = await CardSelectCmd.FromChooseACardScreen(new BlockingPlayerChoiceContext(), cards, Owner);
        if (cardModel != null)
        {
            await ((KnowledgeDemon.IChoosable)cardModel).OnChosen();
        }
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars["s"].UpgradeValueBy(1);
        DynamicVars["d"].UpgradeValueBy(1);
        DynamicVars["t"].UpgradeValueBy(1);
    }
}