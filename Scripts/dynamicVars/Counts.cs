using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace wylder.Scripts.dynamicVars;

public class Counts : DynamicVar
{
    // 在描述中用作占位符的键，推荐添加前缀避免撞车
    public const string Key = "Counts";
    public static readonly string LocKey = Key.ToUpperInvariant();

    public Counts(decimal baseValue) : base(Key, baseValue)
    {
        this.WithTooltip(LocKey);
    }
}