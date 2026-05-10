using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using wylder.Scripts.pools;
using wylder.Scripts.powers;

namespace wylder.Scripts.cards;

[Pool(typeof(WylderCardPool))]
public class WarCry : AshWarModel
{
    // 基础耗能
    private const int energyCost =0;
    // 卡牌类型
    private const CardType type = CardType.Skill;
    // 卡牌稀有度
    private const CardRarity rarity = CardRarity.Common;
    // 目标类型（AnyEnemy表示任意敌人）
    private const TargetType targetType = TargetType.Self;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;
    // 卡牌的基础属性（例如这里是12点伤害）
    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("count", 3)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>(), HoverTipFactory.FromPower<DexterityPower>(), HoverTipFactory.FromKeyword(CardKeyword.Exhaust)];

    public WarCry() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    // 打出时的效果逻辑
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<WarCryStrengthPower>(Owner.Creature, DynamicVars["count"].IntValue, Owner.Creature, this);
        await PowerCmd.Apply<WarCryDexPower>(Owner.Creature, DynamicVars["count"].IntValue, Owner.Creature, this);
        await PowerCmd.Apply<WarCryPower>(Owner.Creature, 1, Owner.Creature, this);
    }

    // 升级后的效果逻辑
    protected override void OnUpgrade()
    {
        DynamicVars["count"].UpgradeValueBy(2m);
    }
}