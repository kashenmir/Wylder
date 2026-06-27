using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using wylder.Scripts.acts;

namespace wylder.Scripts.patchs;

[HarmonyPatch(typeof(ActModel), "get_FilePathIdentifier")]
public class ActModelFilePathIdentifierPatch
{
    static void Postfix(
        ActModel __instance,
        ref string __result
    )
    {
        if (__instance is SoulTree)
        {
            __result = "glory";
        }
    }
}