#if STEAM
using Steamworks;

namespace F00F.NEW;

internal partial class SteamNetwork
{
    public bool Exists => true;

    public int MaxSupportedPlayers => Active ? 250 : 0;
    public string DefaultPlayerName => Active ? GetPlayerName() : null;

    #region Godot

    public sealed override void _EnterTree()
    {
        InitSteam();
        InitOverlay();
    }

    public sealed override void _ExitTree()
    {
        DnitOverlay();
        DnitSteam();
    }

    public sealed override void _Ready()
    {
        SetProcess(Active);
        ActiveChanged += () => SetProcess(Active);
    }

    public sealed override void _Process(double _)
        => SteamAPI.RunCallbacks();

    #endregion

    #region Private

    private AppId_t? _AppId => field ??= SteamUtils.GetAppID();
    private AppId_t AppId => _AppId.Value;

    private static string GetPlayerName() =>
        SteamFriends.GetPlayerNickname(SteamUser.GetSteamID()) ??
        SteamFriends.GetPersonaName() ?? System.Environment.UserName;

    #endregion
}
#endif
