using MegaCrit.Sts2.Core.Models;
using wylder.Scripts.cards;

namespace wylder.Scripts.powers;

public class StarCallerStrenthPower : CustomTemporaryStrengthPower
{
    public override AbstractModel OriginModel => ModelDb.Card<StarCaller>();

    protected override bool IsPositive => false;
    
    public override string? CustomPackedIconPath => "res://wylder/powers/strength_down.png";
    public override string? CustomBigIconPath => "res://wylder/powers/strength_down.png";
}