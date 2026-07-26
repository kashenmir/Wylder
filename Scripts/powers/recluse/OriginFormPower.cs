using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace wylder.Scripts.powers.recluse;

public class OriginFormPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://wylder/powers/origin_form_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/origin_form_power.png";

    public override async Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
    {
        if (creator == Owner.Player)
        {
            if (card.DynamicVars.ContainsKey("Damage"))
            {
                card.DynamicVars.Damage.UpgradeValueBy(card.DynamicVars.Damage.IntValue);
            }
            if (card.DynamicVars.ContainsKey("ExtraDamage"))
            {
                card.DynamicVars.ExtraDamage.UpgradeValueBy(card.DynamicVars.ExtraDamage.IntValue);
            }
            if (card.DynamicVars.ContainsKey("OstyDamage"))
            {
                card.DynamicVars.OstyDamage.UpgradeValueBy(card.DynamicVars.OstyDamage.IntValue);
            }
            if (card.DynamicVars.ContainsKey("Block"))
            {
                card.DynamicVars.Block.UpgradeValueBy(card.DynamicVars.Block.IntValue);
            }
            if (card.DynamicVars.ContainsKey("lightningRod"))
            {
                card.DynamicVars["lightningRod"].UpgradeValueBy(card.DynamicVars["lightningRod"].IntValue);
            }
            if (card.DynamicVars.ContainsKey("aoeDamage"))
            {
                card.DynamicVars["aoeDamage"].UpgradeValueBy(card.DynamicVars["aoeDamage"].IntValue);
            }
            foreach (NHandCardHolder nHandCardHolder in  NCombatRoom.Instance.Ui.Hand.ActiveHolders)
            {
                NCard? cards = nHandCardHolder.CardNode;
                CardModel? cardModel = nHandCardHolder.CardModel;
                if (cards != null && cardModel != null && cardModel==card)
                {
                    cards.UpdateVisuals(PileType.Hand, CardPreviewMode.Normal);
                }
            }
        }
    }
}
