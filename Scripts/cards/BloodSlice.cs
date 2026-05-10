using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using wylder.Scripts.dynamicVars;
using wylder.Scripts.pools;
using wylder.Scripts.powers;

namespace wylder.Scripts.cards;

[Pool(typeof(WylderCardPool))]
public class BloodSlice : AshWarModel
{
    // 基础耗能
    private const int energyCost = 1;
    // 卡牌类型
    private const CardType type = CardType.Attack;
    // 卡牌稀有度
    private const CardRarity rarity = CardRarity.Rare;
    // 目标类型（AnyEnemy表示任意敌人）
    private const TargetType targetType = TargetType.AnyEnemy;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;
    
    // 卡牌的基础属性（例如这里是12点伤害）
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6, ValueProp.Move), new IntVar("blood", 6m), new Chase(1)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<BloodBase>()];

    public BloodSlice() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        int intValue = DynamicVars["blood"].IntValue;
        foreach (PowerModel power in cardPlay.Target.Powers.ToList())
        {
            if (power.Type == PowerType.Debuff && power.StackType == PowerStackType.Counter)
            {
                intValue += power.Amount;
            } else if (power.Type == PowerType.Debuff && power.StackType == PowerStackType.Single)
            {
                intValue += 1;
            }
        }
        await PowerCmd.Apply<BasicChasePower>(Owner.Creature, (int)(intValue*0.5m*DynamicVars["Chase"].IntValue), Owner.Creature, null);
        await PowerCmd.Apply<BloodBase>(cardPlay.Target, DynamicVars["blood"].IntValue, cardPlay.Target, null);
        Frost? frost = cardPlay.Target.GetPower<Frost>();
        if (frost != null)
        {
            await PowerCmd.Remove(frost);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Chase"].UpgradeValueBy(1m);
    }
}