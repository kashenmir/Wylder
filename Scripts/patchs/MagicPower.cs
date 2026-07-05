namespace wylder.Scripts.patchs;

public class MagicPower
{
    public int Count=1;
    
    public MagicPowerOption Option=MagicPowerOption.None;

    // 无参构造（可选）
    public MagicPower()
    {
    }

    // ✔ 有参构造
    public MagicPower(int count, MagicPowerOption option)
    {
        Count = count;
        Option = option;
    }
}