using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using wylder.Scripts.acts;

namespace wylder.Scripts.patchs;

[HarmonyPatch(typeof(ModelDb), "get_Acts")]
public class ModelDbActsPatch
{
    static void Postfix(ref IEnumerable<ActModel> __result)
    {
        Log.Warn("ModelDbActsPatch.Postfix be run,Act4SelectionState.SelectedBoss="+Act4SelectionState.SelectedBoss);
        if (__result is List<ActModel> acts)
        {
            if (!acts.Any(a => a is SoulTree))
            {
                acts.Add(ModelDb.Act<SoulTree>());
            }
        }
    }
}