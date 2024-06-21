#if STEAM

namespace F00F.NEW;

using System;
using Steamworks;

internal partial class SteamNetwork
{
    public bool JoinSession(Network.SessionJoin join, Network.SessionFeed feed)
    {
        if (!Active) { Status.Warn.Push("Steam not active", Name); return false; }
        if (SessionId.IsValid()) { Status.Warn.Push("Already in session", Name); return false; }

        var PlayerName = join.PlayerName.IfNullOrEmpty(DefaultPlayerName);

        InitSession(PlayerName, feed);
        CallResult<LobbyEnter_t>.Create(OnSessionEntered).Set(
            SteamMatchmaking.JoinLobby((CSteamID)join.SessionId));
        return true;

        void OnSessionEntered(LobbyEnter_t t, bool fail)
        {
            var result = (EChatRoomEnterResponse)t.m_EChatRoomEnterResponse;
            if (fail || result is not EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess)
            {
                var msg = result.Trim("k_EChatRoomEnterResponse");
                Status.Error.Push(msg, Name);
                return;
            }

            Status.Success.Push("CONNECTED", Name);
        }
    }
}
#endif
