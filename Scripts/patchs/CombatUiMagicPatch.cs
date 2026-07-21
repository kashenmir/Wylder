using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Combat;
using wylder.Scripts.relics;

namespace wylder.Scripts.patchs;

[HarmonyPatch(typeof(NCombatUi), "Activate")]
public class CombatUiMagicPatch
{
    static void Postfix(NCombatUi __instance, CombatState state)
    {
        // 防止重复创建
        if (__instance.GetNodeOrNull<NMagicUi>("MagicUi") != null)
        {
            return;
        }

        // 获取玩家（和原逻辑一致）
        Player me = LocalContext.GetMe(state);
        if (me == null)
        {
            GD.PrintErr("[Wylder] Player is null, cannot create MagicUi");
            return;
        }

        if (me.Relics.OfType<WitchBrooch>().FirstOrDefault() == null)
        {
            return;
        }

        // 加载UI场景
        var scene = ResourceLoader.Load<PackedScene>(
            "res://wylder/scenes/recluse_magic_ui.tscn"
        );

        if (scene == null)
        {
            GD.PrintErr("[Wylder] Failed to load NMagicUi scene");
            return;
        }

        var ui = scene.Instantiate<NMagicUi>();
        ui.Name = "MagicUi";

        // 固定坐标
        ui.SetAnchorsPreset(
            Control.LayoutPreset.TopLeft
        );
        ui.Position = new Vector2(100, 684);

        // 关键：挂进战斗UI
        __instance.AddChild(ui);

        // 关键：绑定玩家
        ui.Initialize(me);

        GD.Print("[Wylder] NMagicUi injected into Combat UI.");
    }
}