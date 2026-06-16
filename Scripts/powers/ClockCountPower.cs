using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace wylder.Scripts.powers;

public class ClockCountPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://wylder/powers/time_eater_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/time_eater_power.png";

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("max", 12)];

    private int _pointer;

    public override int DisplayAmount => _pointer;

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player)
        {
            return;
        }
        _pointer++;
        InvokeDisplayAmountChanged();
        if (_pointer >= DynamicVars["max"].IntValue)
        {
            _pointer = 0;
            InvokeDisplayAmountChanged();
            PlayerCmd.EndTurn(Owner.Player, canBackOut: false);
        }
    }
}
