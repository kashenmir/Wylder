using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace wylder.Scripts.patchs;

[HarmonyPatch(typeof(RunManager), nameof(RunManager.GenerateRooms))]
public class RunManagerGenerateRoomsPatch
{
    static bool Prefix(RunManager __instance)
    {
        var state = (RunState)AccessTools
            .Property(typeof(RunManager), "State")
            .GetValue(__instance)!;

        List<AncientEventModel> list = state
            .UnlockState
            .SharedAncients
            .ToList()
            .UnstableShuffle(state.Rng.UpFront);

        foreach (ActModel item in state.Acts.Skip(1))
        {
            int count = state.Rng.UpFront.NextInt(list.Count + 1);

            List<AncientEventModel> subset =
                list.Take(count).ToList();

            list = list.Except(subset).ToList();

            item.SetSharedAncientSubset(subset);
        }

        for (int i = 0; i < state.Acts.Count; i++)
        {
            ActModel act = state.Acts[i];

            act.GenerateRooms(
                state.Rng.UpFront,
                state.UnlockState,
                state.Players.Count > 1
            );

            bool shouldApplyTutorial =
                (bool)AccessTools.Method(
                    typeof(RunManager),
                    "ShouldApplyTutorialModifications"
                ).Invoke(__instance, null)!;

            if (shouldApplyTutorial)
            {
                act.ApplyDiscoveryOrderModifications(
                    state.UnlockState
                );
            }

            //
            // 唯一修改的地方
            //
            int count = Act4SelectionState.SelectedBoss == Act4SelectionState.BossOption.None ? 1 : 2;
            if (
                i == state.Acts.Count - count
                && state.AscensionLevel >= (int)AscensionLevel.DoubleBoss
            )
            {
                EncounterModel secondBossEncounter =
                    state.Rng.UpFront.NextItem(
                        act.AllBossEncounters.Where(
                            e => e.Id != act.BossEncounter.Id
                        )
                    );

                act.SetSecondBossEncounter(
                    secondBossEncounter
                );
            }
        }

        return false;
    }
}