using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using wylder.Scripts.cards;
using wylder.Scripts.pools;

namespace wylder.Scripts.relics;

[Pool(typeof(EventRelicPool))]
public class FirePotBundle : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool ShowCounter => false;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("count", 3)];

    public override string PackedIconPath => $"res://wylder/images/relics/{GetType().Name}.png";
    protected override string PackedIconOutlinePath => $"res://wylder/images/relics/{GetType().Name}.png";
    protected override string BigIconPath => $"res://wylder/images/relics/{GetType().Name}.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<FirePot>()];

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (player == Owner && combatState.RoundNumber <= 1)
        {
            Flash();
            for (int i = 0; i < DynamicVars["count"].IntValue; i++)
            {
                CardModel card = combatState.CreateCard<FirePot>(Owner);
                CardCmd.Upgrade(card);
                CardCmd.ApplyKeyword(card, CardKeyword.Retain);
                await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner);
            }
        }
    }
}
