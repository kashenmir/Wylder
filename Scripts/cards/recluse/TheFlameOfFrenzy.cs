using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using wylder.Scripts.keywords;
using wylder.Scripts.pools;

namespace wylder.Scripts.cards.recluse;

[Pool(typeof(RecluseCardPool))]
public class TheFlameOfFrenzy : TestCardModel
{
    private const int energyCost = 0;
    protected override bool HasEnergyCostX => true;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.AllEnemies;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(14, ValueProp.Move)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [MyKeywords.FrenzyFlame];

    public TheFlameOfFrenzy() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int x = ResolveEnergyXValue() + (IsUpgraded ? 1 : 0);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).WithHitCount(x).FromCard(this, cardPlay)
            .TargetingRandomOpponents(CombatState)
            .WithHitFx("vfx/vfx_attack_fire")
            .Execute(choiceContext);

        for (int i = 0; i < x; i++)
        {
            CardModel mad = CombatState.CreateCard<Mad>(Owner);
            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(mad, PileType.Discard, null));
        }
    }

    protected override void OnUpgrade()
    {
    }
}
