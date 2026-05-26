using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace wylder.Scripts.powers;

public class LipulaDebuff3Power : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;
    
    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override string? CustomPackedIconPath => "res://wylder/powers/lipula_debuff_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/lipula_debuff_power.png";

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];
    
    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        if (player != Owner.Player)
        {
            return count;
        }
        return count - DynamicVars.Cards.IntValue;
    }
}