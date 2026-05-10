using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace wylder.Scripts.dynamicVars;

public class MagicBuff : DynamicVar
{
    public const string Key = "MagicBuff";
    public static readonly string LocKey = Key.ToUpperInvariant();

    public MagicBuff(decimal baseValue) : base(Key, baseValue)
    {
        this.WithTooltip(LocKey);
    }
}