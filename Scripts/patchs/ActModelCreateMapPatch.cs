using HarmonyLib;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using wylder.Scripts.acts;

namespace wylder.Scripts.patchs;

[HarmonyPatch(typeof(ActModel), nameof(ActModel.CreateMap))]
public class ActModelCreateMapPatch
{
    static bool Prefix(
        ActModel __instance,
        RunState runState,
        bool replaceTreasureWithElites,
        ref ActMap __result
    )
    {
        if (__instance is SoulTree)
        {
            __result = new SoulTreeActMap(runState);
            return false;
        }

        return true;
    }
}