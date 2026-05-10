using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace wylder.Scripts.powers;

public class PrepareAttackPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override string? CustomPackedIconPath => "res://wylder/powers/prepare_attack_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/prepare_attack_power.png";

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];
    
    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        if (player != Owner.Player)
        {
            return count;
        }
        return count + Amount;
    }
    
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != base.Owner.Player)
        {
            return;
        }
        foreach (CardModel item in PileType.Hand.GetPile(base.Owner.Player).Cards.Where((CardModel c) => c.IsUpgradable))
        {
            CardCmd.Upgrade(item);
        }
        await PowerCmd.Remove(this);
    }
}