using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using wylder.Scripts.pools;
using wylder.Scripts.potions;

namespace wylder.Scripts.relics;

[Pool(typeof(EventRelicPool))]
public class OpalineBubbletearRelic : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool ShowCounter => false;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("PotionSlots", 1)];

    public override string PackedIconPath => $"res://wylder/images/relics/{GetType().Name}.png";
    protected override string PackedIconOutlinePath => $"res://wylder/images/relics/{GetType().Name}.png";
    protected override string BigIconPath => $"res://wylder/images/relics/{GetType().Name}.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPotion<OpalineBubbletear>()];

    public override async Task AfterObtained()
    {
        await PlayerCmd.GainMaxPotionCount(DynamicVars["PotionSlots"].IntValue, Owner);
        PotionModel potion = ModelDb.Potion<OpalineBubbletear>().ToMutable();
        await PotionCmd.TryToProcure(potion, Owner);
    }
}
