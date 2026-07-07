using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using wylder.Scripts.keywords;
using wylder.Scripts.pools;
using wylder.Scripts.powers.recluse;

namespace wylder.Scripts.cards.recluse;

[Pool(typeof(RecluseCardPool))]
public class AncientDeathOfRancor : TestCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("count", 3), new IntVar("chargeCount", 2)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [MyKeywords.Death];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<DeathRancor>()];

    public AncientDeathOfRancor() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int count = DynamicVars["count"].IntValue;

        ChargePower? charge = Owner.Creature.GetPower<ChargePower>();
        if (charge != null && charge.Amount > 0)
        {
            await PowerCmd.Apply<ChargePower>(choiceContext, Owner.Creature, -1, Owner.Creature, this);
            count += DynamicVars["chargeCount"].IntValue;
        }

        for (int i = 0; i < count; i++)
        {
            CardModel cardDraw = CombatState.CreateCard<DeathRancor>(Owner);
            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(cardDraw, PileType.Draw, Owner, CardPilePosition.Random));

            CardModel cardDiscard = CombatState.CreateCard<DeathRancor>(Owner);
            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(cardDiscard, PileType.Discard, null, CardPilePosition.Random));
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["count"].UpgradeValueBy(1m);
    }
}
