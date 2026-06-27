namespace wylder.Scripts.patchs;

public static class Act4SelectionState
{
    public enum BossOption
    {
        None = 0,
        BalanceLawMonster = 1
    }

    public static BossOption SelectedBoss =
        BossOption.None;
}