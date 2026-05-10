using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using wylder.Scripts.pools;
using wylder.Scripts.powers;

namespace wylder.Scripts.cards;

[Pool(typeof(WylderCardPool))]
public class PoisonMoth : AshWarModel
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
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6, ValueProp.Move), new IntVar("poison", 25m)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<PoisonBase>(), HoverTipFactory.FromPower<Poison>()];

    public PoisonMoth() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        if (cardPlay.Target.HasPower<Poison>())
        {
            Poison poison = cardPlay.Target.GetPower<Poison>();
            await PowerCmd.Remove(poison);
            await DamageCmd.Attack(base.DynamicVars["poison"].BaseValue).FromCard(this).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }
        else
        {
            await PowerCmd.Apply<PoisonBase>(cardPlay.Target, 6, cardPlay.Target, null);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["poison"].UpgradeValueBy(7m);
    }
}