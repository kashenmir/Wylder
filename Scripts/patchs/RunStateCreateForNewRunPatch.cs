using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using wylder.Scripts.acts;

namespace wylder.Scripts.patchs;

[HarmonyPatch(typeof(RunState), nameof(RunState.CreateForNewRun))]
public static class RunStateCreateForNewRunPatch
{
    [HarmonyPrefix]
    static void Prefix(
        ref IReadOnlyList<ActModel> acts)
    {
        if (Act4SelectionState.SelectedBoss !=
            Act4SelectionState.BossOption.None)
        {
            return;
        }

        acts = acts
            .Where(a => !(a is SoulTree))
            .ToList();
    }
}