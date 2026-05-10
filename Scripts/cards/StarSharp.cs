using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using wylder.Scripts.pools;

namespace wylder.Scripts.cards;

[Pool(typeof(WylderCardPool))]
public class StarSharp : TestCardModel
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

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(2)];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [EnergyHoverTip];

    public StarSharp() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    // 打出时的效果逻辑
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
    }

    // 升级后的效果逻辑
    protected override void OnUpgrade()
    {
        DynamicVars.Energy.UpgradeValueBy(1m);
    }
}