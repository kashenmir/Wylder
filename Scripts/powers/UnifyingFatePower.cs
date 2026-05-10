using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using wylder.Scripts.cards;

namespace wylder.Scripts.powers;

public class UnifyingFatePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override string? CustomPackedIconPath => "res://wylder/powers/unifying_fate_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/unifying_fate_power.png";

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1), new EnergyVar(1)];
    
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player || cardPlay.Card is not SacredFlask)
        {
            return;
        }
        Flash();
        await CardPileCmd.Draw(new BlockingPlayerChoiceContext(), Amount*DynamicVars.Cards.IntValue, Owner.Player);
        await PlayerCmd.GainEnergy(Amount*DynamicVars.Energy.IntValue, Owner.Player);
    }
}