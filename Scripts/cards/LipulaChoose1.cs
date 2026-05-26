using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;

namespace wylder.Scripts.cards;

[Pool(typeof(TokenCardPool))]
public class LipulaChoose1 : TestCardModel, KnowledgeDemon.IChoosable
{
    // 基础耗能
    private const int energyCost = -1;
    // 卡牌类型
    private const CardType type = CardType.Status;
    // 卡牌稀有度
    private const CardRarity rarity = CardRarity.Status;
    // 目标类型（AnyEnemy表示任意敌人）
    private const TargetType targetType = TargetType.None;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;
    public override bool CanBeGeneratedInCombat => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("s", 2m), new IntVar("d", -4m)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];

    public LipulaChoose1() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }
    
    public async Task OnChosen()
    {
        await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, base.DynamicVars["s"].BaseValue, base.Owner.Creature, this);
        await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, base.DynamicVars["d"].BaseValue, base.Owner.Creature, this);
    }
    
    protected override void OnUpgrade()
    {
    }
}