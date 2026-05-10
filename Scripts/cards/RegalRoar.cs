using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using wylder.Scripts.pools;
using wylder.Scripts.powers;

namespace wylder.Scripts.cards;

[Pool(typeof(WylderCardPool))]
public class RegalRoar : TestCardModel
{
    // 基础耗能
    private const int energyCost = 0;
    // 卡牌类型
    private const CardType type = CardType.Skill;
    // 卡牌稀有度
    private const CardRarity rarity = CardRarity.Rare;
    // 目标类型（AnyEnemy表示任意敌人）
    private const TargetType targetType = TargetType.Self;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("count", 1), new EnergyVar(1)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [EnergyHoverTip];

    public RegalRoar() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    // 打出时的效果逻辑
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (IsUpgraded)
        {
            await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
        }
        await PowerCmd.Apply<RegalRoarPower>(Owner.Creature, 1, Owner.Creature, null);
    }

    // 升级后的效果逻辑
    protected override void OnUpgrade()
    {
    }
}