using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using wylder.Scripts.dynamicVars;
using wylder.Scripts.pools;
using wylder.Scripts.powers;

namespace wylder.Scripts.cards;

[Pool(typeof(WylderCardPool))]
public class SleepMist : AshWarModel
{
    // 基础耗能
    private const int energyCost = 2;
    // 卡牌类型
    private const CardType type = CardType.Skill;
    // 卡牌稀有度
    private const CardRarity rarity = CardRarity.Rare;
    // 目标类型（AnyEnemy表示任意敌人）
    private const TargetType targetType = TargetType.AllEnemies;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;
    // 卡牌的基础属性（例如这里是12点伤害）
    protected override IEnumerable<DynamicVar> CanonicalVars => [new MagicBuff(3m), new IntVar("count", 5)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<SleepBuffPower>(), HoverTipFactory.FromPower<SleepBase>(), HoverTipFactory.FromPower<Sleep>()];

    public SleepMist() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    // 打出时的效果逻辑
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        List<Task> damageTasks = new List<Task>();
        foreach (Creature hittableEnemy in CombatState.HittableEnemies)
        {
            NCreature nCreature = NCombatRoom.Instance?.GetCreatureNode(hittableEnemy);
            if (nCreature != null)
            {
                NGaseousImpactVfx child = NGaseousImpactVfx.Create(nCreature.VfxSpawnPosition, new Color("#D8BFD8"));
                NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(child);
                damageTasks.Add(PowerCmd.Apply<SleepBase>(hittableEnemy, DynamicVars["count"].IntValue, Owner.Creature, this));
            }
        }
        await Task.WhenAll(damageTasks);
        await PowerCmd.Apply<SleepBuffPower>(Owner.Creature, 1, Owner.Creature, null);
    }

    // 升级后的效果逻辑
    protected override void OnUpgrade()
    {
        DynamicVars["count"].UpgradeValueBy(2m);
    }
}