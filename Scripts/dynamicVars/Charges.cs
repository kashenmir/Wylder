using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace wylder.Scripts.dynamicVars;

public class Charges : DynamicVar
{
    // 在描述中用作占位符的键，推荐添加前缀避免撞车
    public const string Key = "Charges";
    // 本地化键，这里设置为大写的Key，也就是"CHARGES"
    public static readonly string LocKey = Key.ToUpperInvariant();

    public Charges(decimal baseValue) : base(Key, baseValue)
    {
        this.WithTooltip(LocKey);
    }
}