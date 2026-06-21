using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace wylder.Scripts.powers;

public class BeastsHuntPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;
    
    public override int DisplayAmount => DynamicVars["count"].IntValue;

    private bool isInActive = true;
    
    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override string? CustomPackedIconPath => "res://wylder/powers/beasts_hunt_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/beasts_hunt_power.png";

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("card", 4), new IntVar("count", 0)];
    
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player || isInActive)
        {
            isInActive = false;
            return;
        }
        DynamicVars["count"].UpgradeValueBy(1);
        if (DynamicVars["count"].IntValue >= DynamicVars["card"].IntValue)
        {
            DynamicVars["count"].UpgradeValueBy(-DynamicVars["count"].IntValue);
            Flash();
            await CardPileCmd.Draw(new BlockingPlayerChoiceContext(), 1, Owner.Player);
        }
        InvokeDisplayAmountChanged();
    }
}