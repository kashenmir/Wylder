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
public class MagmaShot : TestCardModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Basic;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(12, ValueProp.Move)];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [MyKeywords.Lava];

    public MagmaShot() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ChargePower? charge = Owner.Creature.GetPower<ChargePower>();
        bool hasCharge = charge != null && charge.Amount > 0;

        decimal damage = hasCharge ? DynamicVars.Damage.BaseValue + 4 : DynamicVars.Damage.BaseValue;

        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(damage).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        if (hasCharge)
        {
            await PowerCmd.Apply<ChargePower>(choiceContext, Owner.Creature, -1, Owner.Creature, this);
            await PowerCmd.Apply<MagmaBurnPower>(choiceContext, CombatState.HittableEnemies, 2, Owner.Creature, this);
        }
        else
        {
            await PowerCmd.Apply<MagmaBurnPower>(choiceContext, cardPlay.Target, 2, Owner.Creature, this);
        }
    }

    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        if (dealer == Owner.Creature && props.IsPoweredAttack() && cardSource == this)
        {
            await PowerCmd.Apply<PureFirePower>(choiceContext, target, 3, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
    }
}
