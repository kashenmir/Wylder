using wylder.Scripts.pools;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using wylder.Scripts.powers;

namespace wylder.Scripts.cards;

// 加入哪个卡池
[Pool(typeof(WylderCardPool))]
public class BloodBoonRitual : TestCardModel
{
    // 基础耗能
    private const int energyCost = 3;
    // 卡牌类型
    private const CardType type = CardType.Skill;
    // 卡牌稀有度
    private const CardRarity rarity = CardRarity.Rare;
    // 目标类型（AnyEnemy表示任意敌人）
    private const TargetType targetType = TargetType.AllEnemies;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;
    // 卡牌的基础属性（例如这里是12点伤害）
    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("blood", 6m)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<BloodBase>(), HoverTipFactory.FromPower<StrengthPower>()];

    public BloodBoonRitual() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    // 打出时的效果逻辑
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        await Cmd.CustomScaledWait(0.2f, 0.4f);
        for (int i = 0; i < 3; i++)
        {
            List<Task> damageTasks = new List<Task>();
            foreach (Creature hittableEnemy in CombatState.HittableEnemies)
            {
                damageTasks.Add(PowerCmd.Apply<BloodBase>(hittableEnemy, DynamicVars["blood"].IntValue, Owner.Creature, this));
            }
            await Task.WhenAll(damageTasks);
        }
    }

    // 升级后的效果逻辑
    protected override void OnUpgrade()
    {
        DynamicVars["blood"].UpgradeValueBy(3m);
    }
}