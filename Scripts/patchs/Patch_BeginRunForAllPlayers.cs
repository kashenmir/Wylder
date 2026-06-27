using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;
using MegaCrit.Sts2.Core.Multiplayer.Messages.Lobby;

namespace wylder.Scripts.patchs;

// 补丁类
[HarmonyPatch(typeof(StartRunLobby), "BeginRunForAllPlayers")]
public static class Patch_BeginRunForAllPlayers
{
    [HarmonyPrefix]
    public static bool Prefix(
        StartRunLobby __instance,
        string seed,
        List<ModifierModel> modifiers)
    {
        // ❗ 关键：自己完全接管逻辑

        var netService = __instance.NetService;

        if (netService.Type == NetGameType.Client)
            throw new System.InvalidOperationException("Can only begin run as host!");

        // 防重复开始（反射 private _isBeginningRun）
        var isBeginningField = AccessTools.Field(typeof(StartRunLobby), "_isBeginningRun");
        bool isBeginning = (bool)isBeginningField.GetValue(__instance);
        if (isBeginning)
            return false;

        isBeginningField.SetValue(__instance, true);

        // 更新 ascension（调用私有方法）
        AccessTools.Method(typeof(StartRunLobby), "UpdatePreferredAscension")
            ?.Invoke(__instance, null);

        // 构造 message
        var message = new LobbyBeginRunMessage
        {
            playersInLobby = __instance.Players,
            seed = seed,
            modifiers = modifiers.Select(m => m.ToSerializable()).ToList(),
            act1 = __instance.Act1 + "$" + Act4SelectionState.SelectedBoss   // ⭐ 你要的修改就在这里
        };
        Log.Warn("tongbuhost:"+__instance.Act1 + "$" + Act4SelectionState.SelectedBoss);
        // 发包
        netService.SendMessage(message);

        // 调用本地开始逻辑
        AccessTools.Method(typeof(StartRunLobby), "BeginRunLocally")
            ?.Invoke(__instance, new object[] { seed, modifiers });

        // host 关闭
        if (netService.Type == NetGameType.Host)
        {
            var host = netService as INetHostGameService;
            host?.NetHost?.SetHostIsClosed(true);
        }

        return false; // ❗ 不执行原方法
    }
}