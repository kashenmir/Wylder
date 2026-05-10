using wylder.Scripts.pools;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace wylder.Scripts.cards;

[Pool(typeof(WylderCardPool))]
public class Excute : TestCardModel
{
    // 基础耗能
    private const int energyCost = 1;
    // 卡牌类型
    private const CardType type = CardType.Attack;
    // 卡牌稀有度
    private const CardRarity rarity = CardRarity.Common;
    // 目标类型（AnyEnemy表示任意敌人）
    private const TargetType targetType = TargetType.AnyEnemy;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    // 卡牌的基础属性（例如这里是12点伤害）
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(32, ValueProp.Move)];

    public Excute() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    // 打出时的效果逻辑
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        if (!cardPlay.Target.Monster.IntendsToAttack)
        {
            await DamageCmd.Attack(DynamicVars.Damage.IntValue).FromCard(this).Targeting(cardPlay.Target)
                .WithHitFx(null, null, "heavy_attack.mp3")
                .WithHitVfxNode((Creature t) => NBigSlashVfx.Create(t))
                .WithHitVfxNode((Creature t) => NBigSlashImpactVfx.Create(t))
                .Execute(choiceContext);
        }
    }

    // 升级后的效果逻辑
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}