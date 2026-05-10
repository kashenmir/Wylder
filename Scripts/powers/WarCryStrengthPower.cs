using MegaCrit.Sts2.Core.Models;
using wylder.Scripts.cards;

namespace wylder.Scripts.powers;

public class WarCryStrengthPower : CustomTemporaryStrengthPower
{
    public override AbstractModel OriginModel => ModelDb.Card<WarCry>();

    protected override bool IsPositive => true;
    
    public override string? CustomPackedIconPath => "res://wylder/powers/strength_up.png";
    public override string? CustomBigIconPath => "res://wylder/powers/strength_up.png";
}