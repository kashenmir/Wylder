using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Monsters;
using wylder.Scripts.powers;

namespace wylder.Scripts.cards;

// 加入哪个卡池
[Pool(typeof(TokenCardPool))]
public class ChooseDefend: TestCardModel, KnowledgeDemon.IChoosable
{
    // 基础耗能
    private const int energyCost = 0;
    // 卡牌类型
    private const CardType type = CardType.Status;
    // 卡牌稀有度
    private const CardRarity rarity = CardRarity.Status;
    // 目标类型（AnyEnemy表示任意敌人）
    private const TargetType targetType = TargetType.None;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;
    public override bool CanBeGeneratedInCombat => false;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("count", 3m)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<MetallicizePower>()];

    public ChooseDefend() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }
    
    public async Task OnChosen()
    {
        await PowerCmd.Apply<MetallicizePower>(base.Owner.Creature, base.DynamicVars["count"].BaseValue, base.Owner.Creature, this);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars["count"].UpgradeValueBy(1m);
    }
}