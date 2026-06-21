using MegaCrit.Sts2.Core.Models;
using wylder.Scripts.cards;

namespace wylder.Scripts.powers;

public class BlackWavePower : CustomTemporaryStrengthPower
{
    public override AbstractModel OriginModel => ModelDb.Card<BlackWave>();

    protected override bool IsPositive => false;
    
    public override string? CustomPackedIconPath => "res://wylder/powers/strength_down.png";
    public override string? CustomBigIconPath => "res://wylder/powers/strength_down.png";
}