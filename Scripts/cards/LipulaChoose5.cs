using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Monsters;
using wylder.Scripts.powers;

namespace wylder.Scripts.cards;

[Pool(typeof(TokenCardPool))]
public class LipulaChoose5 : TestCardModel, KnowledgeDemon.IChoosable
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

    public LipulaChoose5() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }
    
    public async Task OnChosen()
    {
        CardModel cardModel = CardFactory.GetDistinctForCombat(base.Owner, from c in base.Owner.Character.CardPool.GetUnlockedCards(base.Owner.UnlockState, base.Owner.RunState.CardMultiplayerConstraint)
            where c.Type==CardType.Attack && c.Rarity==CardRarity.Rare 
            select c, 1, base.Owner.RunState.Rng.CombatCardGeneration).FirstOrDefault();
        if (cardModel != null)
        {
            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(cardModel, PileType.Draw, addedByPlayer: true));
        }
        await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), Owner.Creature,
            Owner.Creature.MaxHp - Owner.Creature.CurrentHp * 0.75m, true);
    }
    
    protected override void OnUpgrade()
    {
    }
}