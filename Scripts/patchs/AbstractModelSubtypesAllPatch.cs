// using HarmonyLib;
// using MegaCrit.Sts2.Core.Models;
// using wylder.Scripts.acts;
//
// namespace wylder.Scripts.patchs;
//
// [HarmonyPatch(typeof(AbstractModelSubtypes), "get_All")]
// public class AbstractModelSubtypesAllPatch
// {
//     static void Postfix(ref IEnumerable<Type> __result)
//     {
//         __result = __result.Append(typeof(SoulTree));
//     }
// }