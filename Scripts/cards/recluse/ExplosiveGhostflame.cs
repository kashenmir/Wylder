using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using wylder.Scripts.keywords;
using wylder.Scripts.pools;
using wylder.Scripts.powers;
using wylder.Scripts.powers.recluse;

namespace wylder.Scripts.cards.recluse;

[Pool(typeof(RecluseCardPool))]
public class ExplosiveGhostflame : TestCardModel
{
    private const int energyCost = 4;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.AllEnemies;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(30, ValueProp.Move), new IntVar("deathFlame", 7), new BlockVar(30, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<FrostBase>(),HoverTipFactory.FromPower<Frost>(),HoverTipFactory.FromPower<DeathFlamePower>()];
    
    public override bool GainsBlock => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [MyKeywords.Death];

    public ExplosiveGhostflame() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).TargetingAllOpponents(CombatState)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        await PowerCmd.Apply<DeathFlamePower>(choiceContext, CombatState.HittableEnemies, DynamicVars["deathFlame"].IntValue, Owner.Creature, this);

        ChargePower? charge = Owner.Creature.GetPower<ChargePower>();
        if (charge != null && charge.Amount > 0)
        {
            await PowerCmd.Apply<ChargePower>(choiceContext, Owner.Creature, -1, Owner.Creature, this);
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
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
        DynamicVars.Damage.UpgradeValueBy(10m);
        DynamicVars["deathFlame"].UpgradeValueBy(2m);
        DynamicVars.Block.UpgradeValueBy(10m);
    }
}
