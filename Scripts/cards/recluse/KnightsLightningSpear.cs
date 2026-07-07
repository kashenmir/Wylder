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
using wylder.Scripts.powers.recluse;

namespace wylder.Scripts.cards.recluse;

[Pool(typeof(RecluseCardPool))]
public class KnightsLightningSpear : TestCardModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(9, ValueProp.Move), new IntVar("spearDamage", 4)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [MyKeywords.CapitalDragon];

    public KnightsLightningSpear() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_lightning", "event:/sfx/characters/defect/defect_lightning_evoke", "heavy_attack.mp3")
            .Execute(choiceContext);
        int deBuffAmount = DynamicVars["spearDamage"].IntValue;
        ChargePower? charge = Owner.Creature.GetPower<ChargePower>();
        if (charge != null && charge.Amount > 0)
        {
            await PowerCmd.Apply<ChargePower>(choiceContext, Owner.Creature, -1, Owner.Creature, null);
            deBuffAmount += 3;
        }
        await PowerCmd.Apply<KnightsLightningSpearPower>(choiceContext, cardPlay.Target, deBuffAmount, Owner.Creature, this);
    }

    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        if (dealer == Owner.Creature && props.IsPoweredAttack() && cardSource == this)
        {
            await PowerCmd.Apply<PureLightingPower>(choiceContext, target, 3, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["spearDamage"].UpgradeValueBy(1m);
    }
}
