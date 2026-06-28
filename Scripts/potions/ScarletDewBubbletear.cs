using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.PotionPools;
using wylder.Scripts.pools;
using wylder.Scripts.powers;

namespace wylder.Scripts.potions;

[Pool(typeof(EventPotionPool))]
public class ScarletDewBubbletear : CustomPotionModel
{
    public override PotionRarity Rarity => PotionRarity.Event;

    public override PotionUsage Usage => PotionUsage.CombatOnly;

    public override TargetType TargetType => TargetType.Self;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("count", 20)];

    public override string? CustomPackedImagePath => $"res://wylder/images/potions/{GetType().Name}.png";
    public override string? CustomPackedOutlinePath => $"res://wylder/images/potions/{GetType().Name}.png";

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        if (target != null)
        {
            ModelDb.Potion<ScarletDewBubbletear>();
            await PowerCmd.Apply<ScarletDewPower>(choiceContext, target, 20, Owner.Creature, null);
        }
    }
}
