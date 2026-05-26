using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using wylder.Scripts.dynamicVars;

namespace wylder.Scripts.cards;

[Pool(typeof(TokenCardPool))]
public class Mad : TestCardModel
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

    protected override bool IsPlayable => Owner.PlayerCombatState is { Energy: > 0 };
    
    public override bool CanBeGeneratedInCombat => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1), new Counts(1)];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Exhaust];

    public Mad() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }
    
    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if (card != this)
        {
            return;
        }
        await PlayerCmd.LoseEnergy(1, Owner);
        await CreatureCmd.Damage(choiceContext, Owner.Creature, new DamageVar(3, ValueProp.Unpowered), this);
    }

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card != this || card.Pile == null || card.Pile.Type != PileType.Hand)
        {
            return;
        }
        List<CardModel> cards = PileType.Hand.GetPile(base.Owner.Creature.Player).Cards.Where((CardModel c) => c is Mad).ToList();
        if (cards.Count() >= 3)
        {
            await Cmd.Wait(0.25f);
            foreach (CardModel c in cards)
            {
                await CardCmd.Exhaust(choiceContext, c);
            }
        }
    }

    protected override void OnUpgrade()
    {
    }
}