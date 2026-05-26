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
public class LipulaChoose6 : TestCardModel, KnowledgeDemon.IChoosable
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

    public LipulaChoose6() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }
    
    public async Task OnChosen()
    {
        await PowerCmd.Apply<LipulaDebuff6Power>(Owner.Creature, 1, Owner.Creature, this);
    }
    
    protected override void OnUpgrade()
    {
    }
}