using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace wylder.Scripts.dynamicVars;

public class Chase : DynamicVar
{
    // 在描述中用作占位符的键，推荐添加前缀避免撞车
    public const string Key = "Chase";
    public static readonly string LocKey = Key.ToUpperInvariant();

    public Chase(decimal baseValue) : base(Key, baseValue)
    {
        this.WithTooltip(LocKey);
    }
}