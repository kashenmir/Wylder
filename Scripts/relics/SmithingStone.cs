using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using wylder.Scripts.pools;

namespace wylder.Scripts.relics;

[Pool(typeof(EventRelicPool))]
public class SmithingStone : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool ShowCounter => false;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("count", 2)];

    public override string PackedIconPath => $"res://wylder/images/relics/{GetType().Name}.png";
    protected override string PackedIconOutlinePath => $"res://wylder/images/relics/{GetType().Name}.png";
    protected override string BigIconPath => $"res://wylder/images/relics/{GetType().Name}.png";

    public override async Task AfterObtained()
    {
        List<CardModel> list = (await CardSelectCmd.FromDeckForUpgrade(prefs: new CardSelectorPrefs(CardSelectorPrefs.UpgradeSelectionPrompt, base.DynamicVars["count"].IntValue), player: base.Owner)).ToList();
        foreach (CardModel item in list)
        {
            CardCmd.Upgrade(item);
        }
    }
}
