using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using wylder.Scripts.keywords;
using wylder.Scripts.pools;
using wylder.Scripts.powers;
using wylder.Scripts.powers.recluse;

namespace wylder.Scripts.cards.recluse;

[Pool(typeof(RecluseCardPool))]
public class CarianGreatsword : TestCardModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.AllEnemies;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(18, ValueProp.Move)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [MyKeywords.CarianSword];

    public CarianGreatsword() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).TargetingAllOpponents(CombatState)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        CardPile? discardPile = PileType.Discard.GetPile(Owner);
        if (discardPile != null)
        {
            foreach (CardModel card in discardPile.Cards.ToList())
            {
                if (card != this && card.Keywords.Contains(MyKeywords.CarianSword))
                {
                    await CardPileCmd.Add(card, PileType.Hand);
                }
            }
        }
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
        base.DynamicVars.Damage.UpgradeValueBy(4m);
    }
}
