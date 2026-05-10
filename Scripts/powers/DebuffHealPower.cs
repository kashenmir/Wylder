using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace wylder.Scripts.powers;


public class DebuffHealPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override string? CustomPackedIconPath => "res://wylder/powers/debuff_heal_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/debuff_heal_power.png";
}