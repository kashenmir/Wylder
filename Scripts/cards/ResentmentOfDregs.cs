using wylder.Scripts.pools;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using wylder.Scripts.powers;

namespace wylder.Scripts.cards;

[Pool(typeof(WylderCardPool))]
public class ResentmentOfDregs : TestCardModel
{
    // 基础耗能
    private const int energyCost = 2;
    // 卡牌类型
    private const CardType type = CardType.Power;
    // 卡牌稀有度
    private const CardRarity rarity = CardRarity.Rare;
    // 目标类型（AnyEnemy表示任意敌人）
    private const TargetType targetType = TargetType.Self;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("count", 1)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<RotBase>(),HoverTipFactory.FromPower<Rot>()];
    
    public ResentmentOfDregs() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<ResentmentOfDregsPower>(Owner.Creature, DynamicVars["count"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["count"].UpgradeValueBy(1);
    }
}