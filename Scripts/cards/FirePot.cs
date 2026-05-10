using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using wylder.Scripts.powers;

namespace wylder.Scripts.cards;

[Pool(typeof(TokenCardPool))]
public class FirePot : TestCardModel
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
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(10, ValueProp.Move)];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public FirePot() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    // 打出时的效果逻辑
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await Cmd.CustomScaledWait(0.2f, 0.4f);
        foreach (Creature hittableEnemy in base.CombatState.HittableEnemies)
        {
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NFireSmokePuffVfx.Create(hittableEnemy));
        }
        await Cmd.CustomScaledWait(0.2f, 0.4f);
        await CreatureCmd.Damage(choiceContext, base.CombatState.HittableEnemies, base.DynamicVars.Damage, base.Owner.Creature);
        foreach (Creature hittableEnemy in base.CombatState.HittableEnemies)
        {
            Frost? frost = hittableEnemy.GetPower<Frost>();
            if (frost != null)
            {
                await PowerCmd.Remove(frost);
            }
        }
    }
    
    
    // 升级后的效果逻辑
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4);
    }
}