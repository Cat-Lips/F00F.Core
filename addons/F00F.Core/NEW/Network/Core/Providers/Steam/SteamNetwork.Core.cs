#if STEAM
using Steamworks;

namespace F00F.NEW;

internal partial class SteamNetwork
{
    public bool Exists => true;

    public int MaxSupportedPlayers => Active ? 250 : 0;
    public string DefaultPlayerName => Active ? GetPlayerName() : null;
    public int LocalPlayerId => Active ? (int)UserId.m_SteamID : 0;

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

    private CSteamID? _UserId => field ??= SteamUser.GetSteamID();
    private CSteamID UserId => _UserId.Value;

    private string GetPlayerName() =>
        SteamFriends.GetPlayerNickname(UserId) ??
        SteamFriends.GetPersonaName() ?? System.Environment.UserName;

    #endregion
}
#endif
