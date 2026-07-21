using MegaCrit.Sts2.Core.Models;
using wylder.Scripts.cards.recluse;

namespace wylder.Scripts.powers.recluse;

public class PureLightingDexPower : CustomTemporaryDexPower
{
    public override AbstractModel OriginModel => ModelDb.Card<PureLightning>();

    protected override bool IsPositive => true;
    
    public override string? CustomPackedIconPath => "res://wylder/powers/defend_up.png";
    public override string? CustomBigIconPath => "res://wylder/powers/defend_up.png";
}
