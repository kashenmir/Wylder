using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace wylder.Scripts.patchs;

[HarmonyPatch(typeof(ReattachPower), nameof(ReattachPower.AfterDeath))]
public class ReattachPowerPatch
{
    static bool Prefix(
        ReattachPower __instance,
        PlayerChoiceContext choiceContext,
        Creature creature,
        bool wasRemovalPrevented,
        float deathAnimLength,
        ref Task __result
    )
    {
        __result = MyAfterDeath(
            __instance,
            choiceContext,
            creature,
            wasRemovalPrevented,
            deathAnimLength
        );

        return false;
    }

    static async Task MyAfterDeath(
        ReattachPower __instance,
        PlayerChoiceContext choiceContext,
        Creature creature,
        bool wasRemovalPrevented,
        float deathAnimLength
    )
    {
        if (wasRemovalPrevented || __instance.Owner != creature)
        {
            return;
        }

        var areAllDead = AccessTools
            .Method(typeof(ReattachPower), "AreAllOtherSegmentsDead")
            .Invoke(__instance, null);

        bool allDead = (bool)areAllDead;

        if (!allDead || !__instance.Owner.IsDead)
        {
            var dataType = typeof(ReattachPower)
                .GetNestedType("Data", BindingFlags.NonPublic);

            var getInternalDataMethod = typeof(PowerModel)
                .GetMethod(
                    "GetInternalData",
                    BindingFlags.Instance | BindingFlags.NonPublic
                );

            var genericMethod = getInternalDataMethod!
                .MakeGenericMethod(dataType);

            var data = genericMethod.Invoke(__instance, null);

            dataType
                .GetField("isReviving")!
                .SetValue(data, true);

            if (creature.Monster is DecimillipedeSegment segment)
            {
                // 你修改的地方
                __instance.Owner.Monster.SetMoveImmediate(
                    segment.DeadState,
                    true
                );
            }

            NCombatRoom.Instance?.SetCreatureIsInteractable(
                __instance.Owner,
                on: false
            );
        }
        else
        {
            await Cmd.Wait(0.25f, ignoreCombatEnd: true);

            AccessTools.Method(
                typeof(ReattachPower),
                "DoFadeOutOnAllSegments"
            ).Invoke(__instance, null);
        }
    }
}