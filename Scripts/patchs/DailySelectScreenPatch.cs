using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Screens.DailyRun;

namespace wylder.Scripts.patchs;

[HarmonyPatch(typeof(NDailyRunScreen), "_Ready")]
public class DailySelectScreenPatch
{
    static void Postfix(
        NDailyRunScreen __instance
    )
    {
        // 防止重复创建
        if (
            __instance.GetNodeOrNull<NAct4Selector>(
                "Act4Selector"
            ) != null
        )
        {
            return;
        }

        var scene =
            ResourceLoader.Load<PackedScene>(
                "res://wylder/scenes/Lipula/Act4Selector.tscn"
            );

        if (scene == null)
        {
            GD.PrintErr(
                "[Wylder] Failed to load act4_selector.tscn"
            );
            return;
        }

        var selector =
            scene.Instantiate<NAct4Selector>();

        selector.Name = "Act4Selector";

        // 固定坐标
        selector.SetAnchorsPreset(
            Control.LayoutPreset.TopLeft
        );

        selector.Position =
            new Vector2(1560, 500);

        __instance.AddChild(selector);

        GD.Print(
            "[Wylder] dailyRunScreen Act4Selector added."
        );
    }
}