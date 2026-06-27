using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;
using MegaCrit.Sts2.Core.Multiplayer.Messages.Lobby;

namespace wylder.Scripts.patchs;

[HarmonyPatch(typeof(StartRunLobby))]
public static class StartRunLobbyPatch
{
    // 指定方法名 + 参数类型，即使是私有方法也能精准定位
    [HarmonyTargetMethod]
    static MethodBase TargetMethod()
    {
        return AccessTools.Method(
            typeof(StartRunLobby),
            "HandleLobbyBeginRunMessage",
            new[] { typeof(LobbyBeginRunMessage), typeof(ulong) }
        );
    }

    [HarmonyPrefix]
    static void Prefix(ref LobbyBeginRunMessage message)
    {
        Log.Warn("tongbuchenggong+"+message.act1);
        // 你的分割逻辑，同前
        if (string.IsNullOrEmpty(message.act1) || !message.act1.Contains('$'))
            return;

        var parts = message.act1.Split('$');
        if (parts.Length >= 2)
        {
            message.act1 = parts[0];
            if (Enum.TryParse<Act4SelectionState.BossOption>(parts[1], out var boss))
            {
                Act4SelectionState.SelectedBoss = boss;
            }
        }
    }
}