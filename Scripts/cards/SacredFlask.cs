using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using wylder.Scripts.relics;

namespace wylder.Scripts.cards;

// 加入哪个卡池
[Pool(typeof(TokenCardPool))]
public class SacredFlask : TestCardModel
{
    // 基础耗能
    private const int energyCost = 0;
    // 卡牌类型
    private const CardType type = CardType.Skill;
    // 卡牌稀有度
    private const CardRarity rarity = CardRarity.Token;
    // 目标类型（AnyEnemy表示任意敌人）
    private const TargetType targetType = TargetType.Self;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;
    // 卡牌的基础属性（例如这里是12点伤害）
    protected override IEnumerable<DynamicVar> CanonicalVars => [new HealVar(15m)];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

    protected override bool IsPlayable => Owner.Relics.OfType<SacredFlaskRelic>().FirstOrDefault() != null &&
                                          Owner.Relics.OfType<SacredFlaskRelic>().FirstOrDefault().UseAmount > 0;

    public SacredFlask() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }
    
    public override bool CanBeGeneratedInCombat => false;

    // 打出时的效果逻辑
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        SacredFlaskRelic? relic = Owner.Relics.OfType<SacredFlaskRelic>().FirstOrDefault();
        if (relic != null)
        {
            relic.UseAmount-=1;
        }
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
    }
    
    protected override PileType GetResultPileType()
    {
        PileType resultPileType = base.GetResultPileType();
        if (resultPileType != PileType.Discard)
        {
            return resultPileType;
        }
        return PileType.Hand;
    }
    
    public static async Task<CardModel?> CreateInHand(Player owner, CombatState combatState)
    {
        return (await CreateInHand(owner, 1, combatState)).FirstOrDefault();
    }

    public static async Task<IEnumerable<CardModel>> CreateInHand(Player owner, int count, CombatState combatState)
    {
        if (count == 0)
        {
            return Array.Empty<CardModel>();
        }
        if (CombatManager.Instance.IsOverOrEnding)
        {
            return Array.Empty<CardModel>();
        }
        List<CardModel> cards = new List<CardModel>();
        for (int i = 0; i < count; i++)
        {
            cards.Add(combatState.CreateCard<SacredFlask>(owner));
        }
        await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, addedByPlayer: true);
        return cards;
    }
    
    // 升级后的效果逻辑
    protected override void OnUpgrade()
    {
        DynamicVars.Heal.UpgradeValueBy(3m);
    }
}