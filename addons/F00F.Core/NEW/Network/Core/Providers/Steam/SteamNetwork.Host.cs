#if STEAM

namespace F00F.NEW;

using System;
using Steamworks;

internal partial class SteamNetwork
{
    public bool HostSession(Network.SessionHost host, Network.SessionFeed feed)
    {
        if (!Active) { Status.Warn.Push("Steam not active", Name); return false; }
        if (SessionId.IsValid()) { Status.Warn.Push("Already in session", Name); return false; }

        var SessionScope = Convert(host.SessionScope);
        var MemberLimit = host.MaxPlayers.Clamp(1, MaxSupportedPlayers);
        var PlayerName = host.PlayerName.IfNullOrEmpty(DefaultPlayerName);
        var SessionName = host.SessionName.IfNullOrEmpty(DefaultPlayerName);

        InitSession(PlayerName, feed);
        CallResult<LobbyCreated_t>.Create(OnSessionCreated).Set(
            SteamMatchmaking.CreateLobby(SessionScope, MemberLimit));
        return true;

        void OnSessionCreated(LobbyCreated_t t, bool fail)
        {
            if (fail || t.m_eResult is not EResult.k_EResultOK)
            {
                var msg = t.m_eResult.Trim("k_EResult");
                Status.Error.Push(msg, Name);
                return;
            }

            SessionId = (CSteamID)t.m_ulSteamIDLobby;
            SteamMatchmaking.SetLobbyData(SessionId, Tag.PlayerName, PlayerName);
            SteamMatchmaking.SetLobbyData(SessionId, Tag.SessionName, SessionName);
            SteamMatchmaking.SetLobbyJoinable(SessionId, true);
            Status.Success.Push("ACTIVE", Name);
        }
    }

    #region Utils

    private static ELobbyType Convert(Network.SessionScope scope) => scope switch
    {
        Network.SessionScope.Public => ELobbyType.k_ELobbyTypePublic,
        Network.SessionScope.Friends => ELobbyType.k_ELobbyTypeFriendsOnly,
        Network.SessionScope.InviteOnly => ELobbyType.k_ELobbyTypePrivate,
        _ => throw new NotImplementedException(),
    };

    #endregion
}
#endif
