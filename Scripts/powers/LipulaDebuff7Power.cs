using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace wylder.Scripts.powers;

public class LipulaDebuff7Power : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;
    
    public override string? CustomPackedIconPath => "res://wylder/powers/lipula_debuff_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/lipula_debuff_power.png";

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player)
        {
            return;
        }
        await CreatureCmd.LoseMaxHp(context, Owner, 1, false);
    }
}