using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace wylder.Scripts.patchs;

[HarmonyPatch(typeof(ReattachPower), nameof(ReattachPower.AfterDeath))]
public class ReattachPowerPatch
{
    // 使用 HarmonyPrefix，但返回值改为 Task，参数加上 __result
    [HarmonyPrefix]
    public static Task ReplaceAfterDeath(
        ReattachPower __instance, 
        PlayerChoiceContext choiceContext, 
        Creature creature, 
        bool wasRemovalPrevented, 
        float deathAnimLength)
    {
        // 直接返回你的新逻辑任务，简单粗暴！
        return NewLogic();

        // 封装好的新逻辑
        async Task NewLogic()
        {
            if (wasRemovalPrevented || __instance.Owner != creature)
            {
                return;
            }

            // 绝对能跑通的一句话复刻原版逻辑
            Traverse.Create(__instance).Method("GetInternalData`1").Field("isReviving").SetValue(true);

            if (creature.Monster is DecimillipedeSegment decimillipedeSegment)
            {
                // 加上 true 参数，强制绕过 CanTransitionAway 的检查！
                __instance.Owner.Monster.SetMoveImmediate(decimillipedeSegment.DeadState, true);
            }
            
            NCombatRoom.Instance?.SetCreatureIsInteractable(__instance.Owner, on: false);
        }
    }
}