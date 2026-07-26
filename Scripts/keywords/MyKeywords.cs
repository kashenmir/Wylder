using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace wylder.Scripts.keywords;

public class MyKeywords
{
    [CustomEnum("CHARGES")]
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Charges;
    
    [CustomEnum("ASHWAR")]
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Ashwar;
    
    [CustomEnum("COUNTS")]
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Counts;

    [CustomEnum("CARIAN_SWORD")]
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword CarianSword;

    [CustomEnum("GRAVITY")]
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Gravity;

    [CustomEnum("CRYSTALLIAN")]
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Crystallian;

    [CustomEnum("INVISIBLE")]
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Invisible;

    [CustomEnum("THORNS")]
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Thorns;

    [CustomEnum("ORIGIN")]
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Origin;

    [CustomEnum("GLINTSTONE")]
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Glintstone;

    [CustomEnum("DEATH")]
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Death;

    [CustomEnum("LAVA")]
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Lava;

    [CustomEnum("CAPITAL_DRAGON")]
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword CapitalDragon;

    [CustomEnum("GOLDEN_ORDER")]
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword GoldenOrder;

    [CustomEnum("GIANT_FLAME")]
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword GiantFlame;

    [CustomEnum("SNOW_SORCERY")]
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword SnowSorcery;

    [CustomEnum("FRENZY_FLAME")]
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword FrenzyFlame;

    [CustomEnum("SLY")]
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Sly;
}