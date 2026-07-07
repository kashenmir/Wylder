using BaseLib.Abstracts;

namespace wylder.Scripts.pools;

public class RecluseRelicPool : CustomRelicPoolModel
{
    // 描述中使用的能量图标。大小为24x24。
    public override string? TextEnergyIconPath => "res://wylder/images/energy_test.png";
    // tooltip和卡牌左上角的能量图标。大小为74x74。
    public override string? BigEnergyIconPath => "res://wylder/images/energy_test_big.png";
}