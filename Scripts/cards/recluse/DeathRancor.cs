using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using wylder.Scripts.keywords;
using wylder.Scripts.powers;
using wylder.Scripts.powers.recluse;

namespace wylder.Scripts.cards.recluse;

[Pool(typeof(TokenCardPool))]
public class DeathRancor : TestCardModel
{
    private const int energyCost = -1;
    private const CardType type = CardType.Status;
    private const CardRarity rarity = CardRarity.Token;
    private const TargetType targetType = TargetType.None;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4, ValueProp.Move)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Unplayable, MyKeywords.Death];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DeathFlamePower>()];

    
    public DeathRancor() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }
    
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (cardSource != this)
        {
            return 0m;
        }

        if (target == null)
        {
            return 0m;
        }
        DeathFlamePower? deathFlamePower = target.GetPower<DeathFlamePower>();
        if (deathFlamePower == null)
        {
            return 0m;
        }
        return deathFlamePower.Amount;
    }

    public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? source)
    {
        if (card != this || card.Pile == null || card.Pile.Type != PileType.Hand)
        {
            return;
        }

        await CardCmd.Exhaust(new ThrowingPlayerChoiceContext(), this);
        await CardPileCmd.Draw(new ThrowingPlayerChoiceContext(), 1, Owner);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, null).TargetingRandomOpponents(CombatState)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(new ThrowingPlayerChoiceContext());
    }

    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        if (dealer == Owner.Creature && props.IsPoweredAttack() && cardSource == this)
        {
            await PowerCmd.Apply<PureMagicPower>(choiceContext, target, 3, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
