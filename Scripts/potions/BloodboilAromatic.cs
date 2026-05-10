using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using wylder.Scripts.pools;
using wylder.Scripts.powers;

namespace wylder.Scripts.potions;

[Pool(typeof(WylderPotionPool))]
public class BloodboilAromatic : CustomPotionModel
{
    // 稀有度
    public override PotionRarity Rarity => PotionRarity.Rare;

    // 使用方式，CombatOnly表示只能在战斗中使用。
    public override PotionUsage Usage => PotionUsage.CombatOnly;

    // 目标类型
    public override TargetType TargetType => TargetType.Self;

    // 定义动态变量
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(3)];

    // 药水图片。不一定svg，只要最终能变成Texture的格式就行。
    public override string? CustomPackedImagePath => $"res://wylder/images/potions/{GetType().Name}.png";
    public override string? CustomPackedOutlinePath => $"res://wylder/images/potions/{GetType().Name}.png";
    
    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        if (target != null)
        {
            await PowerCmd.Apply<BloodboilAromaticPower>(target, 3, Owner.Creature, null);
        }
    }
}