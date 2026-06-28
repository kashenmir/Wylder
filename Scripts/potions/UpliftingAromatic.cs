using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using wylder.Scripts.pools;

namespace wylder.Scripts.potions;

[Pool(typeof(WylderPotionPool))]
public class UpliftingAromatic : CustomPotionModel
{
    // 稀有度
    public override PotionRarity Rarity => PotionRarity.Uncommon;

    // 使用方式，CombatOnly表示只能在战斗中使用。
    public override PotionUsage Usage => PotionUsage.CombatOnly;

    // 目标类型
    public override TargetType TargetType => TargetType.AllAllies;

    // 定义动态变量
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(3)];

    // 这里显示预览卡牌灵魂。或者你可以添加提示关键词
    public override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<SlipperyPower>(), HoverTipFactory.FromPower<StrengthPower>()];

    // 药水图片。不一定svg，只要最终能变成Texture的格式就行。
    public override string? CustomPackedImagePath => $"res://wylder/images/potions/{GetType().Name}.png";
    public override string? CustomPackedOutlinePath => $"res://wylder/images/potions/{GetType().Name}.png";
    
    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        if (Owner.Creature.CombatState!=null)
        {
            await PowerCmd.Apply<SlipperyPower>(choiceContext, Owner.Creature.CombatState.Allies, 1, Owner.Creature, null);
            await PowerCmd.Apply<StrengthPower>(choiceContext, Owner.Creature.CombatState.Allies, 1, Owner.Creature, null);
        }
    }
}