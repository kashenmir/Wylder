using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using wylder.Scripts.cards;
using wylder.Scripts.pools;

namespace wylder.Scripts.relics;

// 加入哪个遗物池，此处为通用
[Pool(typeof(WylderRelicPool))]
public class SacredFlaskRelic : CustomRelicModel
{
    // 稀有度
    public override RelicRarity Rarity => RelicRarity.Starter;
    
    public override bool ShowCounter => true;

    // 遗物的数值。替换本地化中的{Cards}。
    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("count", 3m), new IntVar("max", 3m)];
    
    private int _useAmount = 3;

    private int _totalAmount = 3;
    
    private int _activeAct = -1;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<SacredFlask>()];
    
    // 小图标（原版85x85）
    public override string PackedIconPath => $"res://wylder/images/relics/{GetType().Name}.png";
    // 轮廓图标（原版85x85）
    protected override string PackedIconOutlinePath => $"res://wylder/images/relics/{GetType().Name}.png";
    // 大图标（原版256x256）
    protected override string BigIconPath => $"res://wylder/images/relics/{GetType().Name}.png";
    
    public override int DisplayAmount
    {
        get
        {
            return UseAmount;
        }
    }
    
    
    [SavedProperty]
    public int UseAmount
    {
        get
        {
            return _useAmount;
        }
        set
        {
            AssertMutable();
            _useAmount = value;
            DynamicVars["count"].UpgradeValueBy(value-DynamicVars["count"].IntValue);
            UpdateDisplay();
        }
    }
    
    [SavedProperty]
    public int TotalAmount
    {
        get
        {
            return _totalAmount;
        }
        set
        {
            AssertMutable();
            _totalAmount = value;
            DynamicVars["max"].UpgradeValueBy(value-DynamicVars["max"].IntValue);
            UpdateDisplay();
        }
    }
    
    [SavedProperty]
    public int ActiveAct
    {
        get
        {
            return _activeAct;
        }
        set
        {
            AssertMutable();
            _activeAct = value;
        }
    }
    
    private void UpdateDisplay()
    {
        if (UseAmount == 0)
        {
            base.Status = RelicStatus.Normal;
        }
        else
        {
            base.Status = RelicStatus.Active;
        }
        InvokeDisplayAmountChanged();
    }
    
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner == Owner && (cardPlay.Card is SacredFlask || cardPlay.Card is CravingFlask))
        { 
            UpdateDisplay();
        }
    }

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (player == Owner && combatState.RoundNumber <= 1)
        {
            Flash();
            await SacredFlask.CreateInHand(Owner, 1, combatState);
        }
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is RestSiteRoom)
        {
            Flash();
            UseAmount = TotalAmount;
        }

        if (ActiveAct < base.Owner.RunState.CurrentActIndex)
        {
            ActiveAct = base.Owner.RunState.CurrentActIndex;
            Flash();
            UseAmount = TotalAmount;
        }
        return Task.CompletedTask;
    }
    // 初始遗物的升级可以写这里
    // public override RelicModel? GetUpgradeReplacement() => ModelDb.Relic<Circlet>().ToMutable();
    public override RelicModel? GetUpgradeReplacement() => ModelDb.Relic<SacredFlaskRelicNew>(); 
}