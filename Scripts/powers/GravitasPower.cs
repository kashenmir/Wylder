using MegaCrit.Sts2.Core.Models;
using wylder.Scripts.cards;

namespace wylder.Scripts.powers;

public class GravitasPower : CustomTemporaryStrengthPower
{
    public override AbstractModel OriginModel => ModelDb.Card<Gravitas>();

    protected override bool IsPositive => false;
}