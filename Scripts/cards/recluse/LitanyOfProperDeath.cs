using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using wylder.Scripts.keywords;
using wylder.Scripts.pools;
using wylder.Scripts.powers.recluse;

namespace wylder.Scripts.cards.recluse;

[Pool(typeof(RecluseCardPool))]
public class LitanyOfProperDeath : TestCardModel
{
    private const int energyCost = 0;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.AllEnemies;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("strLoss", 1), new IntVar("holy", 3), new IntVar("minionHpLoss", 100)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [MyKeywords.GoldenOrder];

    public LitanyOfProperDeath() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        foreach (Creature enemy in CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<StrengthPower>(choiceContext, enemy, -DynamicVars["strLoss"].IntValue, Owner.Creature, this);
            await PowerCmd.Apply<PureHolyPower>(choiceContext, enemy, DynamicVars["holy"].IntValue, Owner.Creature, this);
            if (enemy.GetPower<MinionPower>() != null)
            {
                await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), enemy, DynamicVars["minionHpLoss"].IntValue, ValueProp.Unblockable | ValueProp.Unpowered, null, null);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["holy"].UpgradeValueBy(2m);
    }
}
