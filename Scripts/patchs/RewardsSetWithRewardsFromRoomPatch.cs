using HarmonyLib;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;

namespace wylder.Scripts.patchs;

[HarmonyPatch(typeof(RewardsSet), nameof(RewardsSet.WithRewardsFromRoom))]
public class RewardsSetWithRewardsFromRoomPatch
{
    static bool Prefix(
        RewardsSet __instance,
        AbstractRoom room,
        ref RewardsSet __result)
    {
        __result = MyWithRewardsFromRoom(
            __instance,
            room
        );

        return false;
    }

    static RewardsSet MyWithRewardsFromRoom(
        RewardsSet __instance,
        AbstractRoom room)
    {
        AccessTools.PropertySetter(
            typeof(RewardsSet),
            nameof(RewardsSet.Room)
        )?.Invoke(__instance, new object[] { room });

        bool isGloryBoss =
            room.RoomType == RoomType.Boss
            && __instance.Player.RunState.CurrentActIndex == 2;

        bool isLastActBoss =
            room.RoomType == RoomType.Boss
            && __instance.Player.RunState.CurrentActIndex >=
               __instance.Player.RunState.Acts.Count - 1;

        if (isGloryBoss || isLastActBoss)
        {
            return __instance;
        }

        var tryGenerateTutorialRewards =
            AccessTools.Method(
                typeof(RewardsSet),
                "TryGenerateTutorialRewards"
            );

        bool generatedTutorialRewards =
            (bool)tryGenerateTutorialRewards.Invoke(
                __instance,
                new object[]
                {
                    __instance.Player,
                    room
                }
            );

        if (!generatedTutorialRewards)
        {
            var generateRewardsFor =
                AccessTools.Method(
                    typeof(RewardsSet),
                    "GenerateRewardsFor"
                );

            var rewards =
                (IEnumerable<Reward>)generateRewardsFor.Invoke(
                    __instance,
                    new object[]
                    {
                        __instance.Player,
                        room
                    }
                );

            __instance.Rewards.AddRange(rewards);
        }

        if (
            __instance.Room is CombatRoom combatRoom
            && combatRoom.ExtraRewards.TryGetValue(
                __instance.Player,
                out List<Reward> value
            )
        )
        {
            __instance.Rewards.AddRange(value);
        }

        return __instance;
    }
}