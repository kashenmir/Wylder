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
public class ImpenetrableThorns : TestCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.None;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6, ValueProp.Move), new IntVar("blood", 6)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [MyKeywords.Thorns];

    public ImpenetrableThorns() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int hitCount = (int)(Owner.Creature.Block / 10);
        if (hitCount > 0)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).WithHitCount(hitCount).FromCard(this, cardPlay)
                .TargetingRandomOpponents(CombatState)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }
    }

    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        if (dealer == Owner.Creature && props.IsPoweredAttack() && cardSource == this)
        {
            await PowerCmd.Apply<BloodBase>(choiceContext, target, DynamicVars["blood"].IntValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
        DynamicVars["blood"].UpgradeValueBy(1m);
    }
}
