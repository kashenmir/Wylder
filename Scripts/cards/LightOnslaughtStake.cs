using wylder.Scripts.pools;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using wylder.Scripts.dynamicVars;
using wylder.Scripts.powers;
using wylder.Scripts.relics;

namespace wylder.Scripts.cards;

[Pool(typeof(WylderCardPool))]
public class LightOnslaughtStake : TestCardModel
{
    // 基础耗能
    private const int energyCost = 1;
    // 卡牌类型
    private const CardType type = CardType.Attack;
    // 卡牌稀有度
    private const CardRarity rarity = CardRarity.Uncommon;
    // 目标类型（AnyEnemy表示任意敌人）
    private const TargetType targetType = TargetType.AllEnemies;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;
    
    //
    private const int maxPower = 60;
    
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
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(24, ValueProp.Move), new Charges(maxPower), new IntVar("count", maxPower)];

    public LightOnslaughtStake() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
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
            (DeckVersion as LightOnslaughtStake)?.UpdateCharge();
            if (DynamicVars["count"].IntValue >= DynamicVars["Charges"].IntValue)
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
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(CombatState)
            .WithHitFx("vfx/vfx_attack_blunt", null, "heavy_attack.mp3")
            .Execute(choiceContext);
        foreach (Creature hittableEnemy in base.CombatState.HittableEnemies)
        {
            Frost? frost = hittableEnemy.GetPower<Frost>();
            if (frost != null)
            {
                await PowerCmd.Remove(frost);
            }
        }
        ClearCharge();
        (DeckVersion as LightOnslaughtStake)?.ClearCharge();
        await CardCmd.Exhaust(choiceContext,this, false, false);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(6m);
    }
}
