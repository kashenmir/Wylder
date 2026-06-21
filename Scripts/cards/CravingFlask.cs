using wylder.Scripts.pools;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Saves.Runs;
using wylder.Scripts.relics;

namespace wylder.Scripts.cards;

// 加入哪个卡池
[Pool(typeof(WylderCardPool))]
public class CravingFlask : TestCardModel
{
    public const int maxPlays = 5;
    private const string _playsKey = "plays";

    private int _playsSeen;
    // 基础耗能
    private const int energyCost = 1;
    // 卡牌类型
    private const CardType type = CardType.Skill;
    // 卡牌稀有度
    private const CardRarity rarity = CardRarity.Rare;
    // 目标类型（AnyEnemy表示任意敌人）
    private const TargetType targetType = TargetType.Self;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;
    
    public override bool CanBeGeneratedInCombat => false;

    // 卡牌的基础属性（例如这里是12点伤害）
    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("plays", maxPlays)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<WeakPower>(), HoverTipFactory.FromKeyword(CardKeyword.Exhaust)];

    public CravingFlask() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }
    
    [SavedProperty]
    public int playsSeen
    {
        get
        {
            return _playsSeen;
        }
        set
        {
            AssertMutable();
            _playsSeen = value;
            DynamicVars["plays"].BaseValue = maxPlays - playsSeen;
        }
    }

    // 打出时的效果逻辑
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        updatePlaysSeen();
        (DeckVersion as CravingFlask)?.updatePlaysSeen();
        await PowerCmd.Apply<WeakPower>(choiceContext, Owner.Creature, 1, Owner.Creature, null);
        if (playsSeen == maxPlays)
        {
            VfxCmd.PlayFullScreenInCombat("vfx/vfx_dramatic_entrance_fullscreen", Owner.Creature);
            await CardCmd.Exhaust(choiceContext,this, false, false);
            SacredFlaskRelic? relic = Owner.Relics.OfType<SacredFlaskRelic>().FirstOrDefault();
            SacredFlaskRelicNew? relicNew = Owner.Relics.OfType<SacredFlaskRelicNew>().FirstOrDefault();
            if (relic != null)
            {
                relic.TotalAmount+=1;
                relic.UseAmount++;
            } 
            else
            {
                await RelicCmd.Obtain<SacredFlaskRelic>(Owner);
            }
            if (relicNew != null)
            {
                relicNew.TotalAmount+=1;
                relicNew.UseAmount++;
            } 
            (DeckVersion as CravingFlask)?.RemoveFromCurrentPile();
        }
    }

    private void updatePlaysSeen()
    {
        playsSeen++;
    }

    // 升级后的效果逻辑
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}