using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace wylder.Scripts.powers;

public class LipulaCurseBasePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Single;
    
    public override string? CustomPackedIconPath => "res://wylder/powers/lipula_curse_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/lipula_curse_power.png";
}
