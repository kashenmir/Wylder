using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using wylder.Scripts.powers;
using wylder.Scripts.powers.recluse;

namespace wylder.Scripts.cards.recluse;

[Pool(typeof(TokenCardPool))]
public class MagicLightningHoly : TestCardModel
{
    private const int energyCost = 0;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Token;
    private const TargetType targetType = TargetType.AllEnemies;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(6, ValueProp.Move), new IntVar("frost", 4), new IntVar("count", 5)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<FrostBase>(), HoverTipFactory.FromPower<Frost>()];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public MagicLightningHoly() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int frostAmount = DynamicVars["frost"].IntValue;
        int hitCount = DynamicVars["count"].IntValue;
        for (int i = 0; i < hitCount; i++)
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
            List<Task> damageTasks = new List<Task>();
            foreach (Creature hittableEnemy in CombatState.HittableEnemies)
            {
                NCreature nCreature = NCombatRoom.Instance?.GetCreatureNode(hittableEnemy);
                if (nCreature != null)
                {
                    NGaseousImpactVfx child = NGaseousImpactVfx.Create(nCreature.VfxSpawnPosition, new Color("#84A7A9"));
                    NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(child);
                    damageTasks.Add(PowerCmd.Apply<FrostBase>(choiceContext, hittableEnemy, frostAmount, Owner.Creature, this));
                }
            }
            await Task.WhenAll(damageTasks);
        }

        PlayerCmd.EndTurn(Owner, canBackOut: false);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars["count"].UpgradeValueBy(1m);
    }
}
