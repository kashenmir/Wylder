using wylder.Scripts.pools;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace wylder.Scripts.cards;

public abstract class TestCardModel : CustomCardModel
{
    public override string PortraitPath => $"res://wylder/images/cards/{GetType().Name}.png";

    public TestCardModel(int energyCost, CardType type, CardRarity rarity, TargetType targetType, bool shouldShowInCardLibrary) : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }
}