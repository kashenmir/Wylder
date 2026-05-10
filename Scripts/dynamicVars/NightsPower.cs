using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace wylder.Scripts.dynamicVars;

public class NightsPower : DynamicVar
{
    public const string Key = "NightsPower";
    public static readonly string LocKey = Key.ToUpperInvariant();

    public NightsPower(decimal baseValue) : base(Key, baseValue)
    {
        this.WithTooltip(LocKey);
    }
}