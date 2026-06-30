using wylder.Scripts.pools;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Saves.Runs;
using wylder.Scripts.dynamicVars;
using wylder.Scripts.powers;
using wylder.Scripts.relics;

namespace wylder.Scripts.cards;

[Pool(typeof(WylderCardPool))]
public class ClawJump : TestCardModel
{
    // 基础耗能
    private const int energyCost = 0;
    // 卡牌类型
    private const CardType type = CardType.Skill;
    // 卡牌稀有度
    private const CardRarity rarity = CardRarity.Uncommon;
    // 目标类型（AnyEnemy表示任意敌人）
    private const TargetType targetType = TargetType.Self;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;
    
    //
    private const int maxPower = 36;
    
    protected override bool IsPlayable => ChargeCount >= maxPower;

    private int _chargeCount = maxPower;
    
    [SavedProperty]
    public int ChargeCount
    {
        get
        {
            return _chargeCount;
        }
        set
        {
            AssertMutable();
            _chargeCount = value;
            DynamicVars["count"].UpgradeValueBy(value-DynamicVars["count"].IntValue);
        }
    }
    
    public void UpdateCharge()
    {
        if (ChargeCount < DynamicVars["Charges"].IntValue)
        {
            Lune? lune = Owner.Relics.OfType<Lune>().FirstOrDefault();
            if (lune != null)
            {
                ChargeCount += 3;
            }
            else
            {
                ChargeCount += 2;
            }
        }
    }

    public void ClearCharge()
    {
        ChargeCount = 0;
    }

    // 卡牌的基础属性（例如这里是20点伤害）
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2), new Charges(maxPower), new IntVar("count", maxPower)];
    
    public ClawJump() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }
    
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner)
        {
            return;
        }

        if (cardPlay.Card == this)
        {
            return;
        }
        
        CardPile? pile = Pile;
        if (pile != null)
        {
            UpdateCharge();
            (DeckVersion as ClawJump)?.UpdateCharge();
            if (DynamicVars["count"].IntValue >= DynamicVars["Charges"].IntValue && pile.Type == PileType.Exhaust)
            {
                await CardPileCmd.Add(this, PileType.Hand);
            }
        }
        return;
    }
    
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (player == Owner && combatState.RoundNumber <= 1 && ChargeCount<maxPower)
        {
            await CardCmd.Exhaust(choiceContext,this, false, false);
        }
    }
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
        ClearCharge();
        (DeckVersion as ClawJump)?.ClearCharge();
        await CardCmd.Exhaust(choiceContext,this, false, false);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}