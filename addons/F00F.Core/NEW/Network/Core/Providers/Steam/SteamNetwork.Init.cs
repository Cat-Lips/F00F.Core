#if STEAM
using System.IO;
using Steamworks;

namespace F00F.NEW;

internal partial class SteamNetwork
{
    private void InitSteam()
    {
        LoadLib();
        ActivateAPI();
    }

    private void DnitSteam()
    {
        ExitSession();
        ShutdownAPI();
        UnloadLib();
    }

    #region LIB

    private NativeLib lib;
    private void LoadLib()
    {
        lib = NativeLib.Load(LibName(), out var error);
        if (lib is null) Status.Error.Push($"Failed to load Steam API: {error}", Name);

        static string LibName() =>
#if GODOT_WINDOWS
            "steam_api64.dll";
#elif GODOT_LINUXBSD
            "libsteam_api.so";
#elif GODOT_MACOS
            "libsteam_api.dylib";
#else
            null;
#endif
    }

    private void UnloadLib()
    {
        lib?.Dispose();
        lib = null;
    }

    #endregion

    #region API

    private void ActivateAPI()
    {
        if (lib is null) return;

        var result = SteamAPI.InitEx(out var message);
        if (result is not ESteamAPIInitResult.k_ESteamAPIInitResult_OK)
        {
            Status.Error.Push(result.Trim("k_ESteamAPIInitResult_"), Name);
            if (message.NotNullOrEmpty()) Log.Error(message);
        }
        else
        {
            if (TryRestartThroughSteam()) return;
            SteamNetworkingUtils.InitRelayNetworkAccess();
            Active = true; Status.Success.Push("ACTIVE", Name);
        }

        Log.Debug("Missing steam_appid.txt", @if: !File.Exists("steam_appid.txt"));
    }

    private void ShutdownAPI()
    {
        if (!Active) return;

        SteamAPI.Shutdown();
        Active = false;
    }

    #region Utils

    private bool TryRestartThroughSteam()
    {
        if (Editor.IsEditor) return false;
        if (!SteamAPI.RestartAppIfNecessary(AppId)) return false;
        Status.Info.Push("Restarting through Steam", Name);
        GetTree().Quit();
        return true;
    }

    #endregion

    #endregion
}
#endif
