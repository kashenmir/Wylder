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
public class BriarsOfPunishment : TestCardModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.AllEnemies;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(5, ValueProp.Move), new IntVar("chargeDamage", 7)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [MyKeywords.Thorns];

    public BriarsOfPunishment() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner.Creature, 1, ValueProp.Unpowered, null, null);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).WithHitCount(2).FromCard(this, cardPlay).TargetingAllOpponents(CombatState)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        await PowerCmd.Apply<BloodBase>(choiceContext, CombatState.HittableEnemies, DynamicVars.Damage.IntValue * 2, Owner.Creature, this);

        ChargePower? charge = Owner.Creature.GetPower<ChargePower>();
        if (charge != null && charge.Amount > 0)
        {
            await PowerCmd.Apply<ChargePower>(choiceContext, Owner.Creature, -1, Owner.Creature, this);
            await DamageCmd.Attack(DynamicVars["chargeDamage"].IntValue).FromCard(this, cardPlay).TargetingAllOpponents(CombatState)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
            await PowerCmd.Apply<BloodBase>(choiceContext, CombatState.HittableEnemies, DynamicVars["chargeDamage"].IntValue, Owner.Creature, this);
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
        DynamicVars.Damage.UpgradeValueBy(1m);
    }
}
