using HarmonyLib;
using MegaCrit.Sts2.Core.Models.Singleton;

namespace wylder.Scripts.patchs;

[HarmonyPatch(typeof(MultiplayerScalingModel), nameof(MultiplayerScalingModel.GetMultiplayerScaling))]
public static class MultiplayerScalingModelPatch
{
    [HarmonyPrefix]
    public static void Prefix(ref int actIndex)
    {
        if (actIndex != 0 && actIndex != 1 && actIndex != 2)
        {
            actIndex = 2;
        }
    }
}