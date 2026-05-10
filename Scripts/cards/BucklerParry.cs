using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.ValueProps;
using wylder.Scripts.pools;
using wylder.Scripts.powers;

namespace wylder.Scripts.cards;

[Pool(typeof(WylderCardPool))]
public class BucklerParry : TestCardModel, ITranscendenceCard
{
    // 基础耗能
    private const int energyCost = 1;
    // 卡牌类型
    private const CardType type = CardType.Skill;
    // 卡牌稀有度
    private const CardRarity rarity = CardRarity.Basic;
    // 目标类型（AnyEnemy表示任意敌人）
    private const TargetType targetType = TargetType.Self;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;
    public override bool CanBeGeneratedInCombat => false;
    // 卡牌的基础属性（例如这里是12点伤害）
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5, ValueProp.Move)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [StunIntent.GetStaticHoverTip()];
    
    public override bool GainsBlock => true;

    public BucklerParry() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    // 打出时的效果逻辑
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        await PowerCmd.Apply<BucklerParryPower>(Owner.Creature, 1, Owner.Creature, null);
    }

    // 升级后的效果逻辑
    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
    }
    
    public CardModel GetTranscendenceTransformedCard() => ModelDb.Card<StormParry>();
}