#if STEAM
using System.Collections.Generic;
using System.Linq;
using Godot;
using Steamworks;

namespace F00F.NEW;

internal partial class SteamNetwork
{
    #region Tags

    private static class Tag
    {
        public const string GameName = "GameName";
        public const string PlayerName = "PlayerName";
        public const string SessionHost = "SessionHost";
        public const string SessionName = "SessionName";
        public const string PingLocation = "PingLocation";
        public const string PlayerLocation = "PlayerLocation";
        public const string SessionLocation = "SessionLocation";
    }

    #endregion

    #region Private

    private CSteamID SessionId;

    private Callback<LobbyEnter_t> SessionReady;
    private Callback<LobbyDataUpdate_t> DataUpdate;
    private Callback<LobbyChatUpdate_t> MemberUpdate;
    private Callback<SteamNetworkingMessagesSessionFailed_t> MsgFail;
    private Callback<SteamNetworkingMessagesSessionRequest_t> MsgInit;

    #endregion

    private void InitSession(string PlayerName, Network.SessionFeed feed)
    {
        SessionReady = Callback<LobbyEnter_t>.Create(OnSessionReady);
        DataUpdate = Callback<LobbyDataUpdate_t>.Create(OnDataUpdate);
        MemberUpdate = Callback<LobbyChatUpdate_t>.Create(OnMemberUpdate);
        MsgFail = Callback<SteamNetworkingMessagesSessionFailed_t>.Create(OnMsgFail);
        MsgInit = Callback<SteamNetworkingMessagesSessionRequest_t>.Create(OnMsgInit);

        void OnSessionReady(LobbyEnter_t t)
        {
            var response = (EChatRoomEnterResponse)t.m_EChatRoomEnterResponse;
            if (response is not EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess)
            {
                var msg = response.Trim("k_EChatRoomEnterResponse");
                Status.Error.Push(msg, Name);
                return;
            }

            SessionId = (CSteamID)t.m_ulSteamIDLobby;
            SteamMatchmaking.SetLobbyMemberData(SessionId, Tag.PlayerName, PlayerName);
            SteamMatchmaking.SetLobbyMemberData(SessionId, Tag.PingLocation, GetPingLocation());
            SteamMatchmaking.SetLobbyMemberData(SessionId, Tag.PlayerLocation, SteamUtils.GetIPCountry());
        }

        void OnDataUpdate(LobbyDataUpdate_t t)
        {
            var playerId = (CSteamID)t.m_ulSteamIDMember;
            var sessionId = (CSteamID)t.m_ulSteamIDLobby;
            if (SessionId != sessionId) return;

            if (playerId == SessionId)
                DoSessionUpdate();
            else DoPlayerUpdate();

            void DoSessionUpdate()
            {
                var SessionHost = SteamMatchmaking.GetLobbyData(SessionId, Tag.SessionHost);
                var SessionName = SteamMatchmaking.GetLobbyData(SessionId, Tag.SessionName);
                var SessionPing = GetPlayerPing(SteamMatchmaking.GetLobbyData(sessionId, Tag.PingLocation));
                var SessionLocn = SteamMatchmaking.GetLobbyData(sessionId, Tag.SessionLocation);
                var MemberCount = SteamMatchmaking.GetNumLobbyMembers(SessionId);
                var MemberLimit = SteamMatchmaking.GetLobbyMemberLimit(SessionId);
                feed.SessionUpdate?.Invoke(new(SessionId.m_SteamID, SessionHost, SessionName, SessionLocn, SessionPing, MemberCount, MemberLimit));
            }

            void DoPlayerUpdate()
            {
                var PlayerName = SteamMatchmaking.GetLobbyMemberData(sessionId, playerId, Tag.PlayerName);
                var PlayerAvatar = LoadImage(SteamFriends.GetSmallFriendAvatar(playerId));
                var PingLocation = SteamMatchmaking.GetLobbyMemberData(sessionId, playerId, Tag.PingLocation);
                var PlayerLocation = SteamMatchmaking.GetLobbyMemberData(sessionId, playerId, Tag.PlayerLocation);
                feed.PlayerUpdate?.Invoke(new(playerId.m_SteamID, PlayerName, PlayerAvatar, PlayerLocation, GetPlayerPing(PingLocation)));
            }
        }

        void OnMemberUpdate(LobbyChatUpdate_t t)
        {
            var change = (EChatMemberStateChange)t.m_rgfChatMemberStateChange;
            var playerId = t.m_ulSteamIDUserChanged;

            switch (change)
            {
                case EChatMemberStateChange.k_EChatMemberStateChangeEntered:
                    feed.PlayerAdd?.Invoke(playerId);
                    break;

                case EChatMemberStateChange.k_EChatMemberStateChangeLeft:
                case EChatMemberStateChange.k_EChatMemberStateChangeKicked:
                case EChatMemberStateChange.k_EChatMemberStateChangeBanned:
                case EChatMemberStateChange.k_EChatMemberStateChangeDisconnected:
                    var reason = change.Trim("k_EChatMemberStateChange");
                    feed.PlayerRemove?.Invoke(playerId, reason);
                    break;
            }
        }

        void OnMsgInit(SteamNetworkingMessagesSessionRequest_t t)
        {
            var playerId = t.m_identityRemote.GetSteamID();
            if (MemberList().Contains(playerId))
            {
                if (SteamNetworkingMessages.AcceptSessionWithUser(ref t.m_identityRemote))
                    Log.Info($"Accepted: {playerId}");
                else Log.Warn($"ACCEPT ERROR: {playerId}");
            }
            else
            {
                if (SteamNetworkingMessages.CloseSessionWithUser(ref t.m_identityRemote))
                    Log.Info($"Rejected: {playerId}");
                else Log.Warn($"REJECT ERROR: {playerId}");
            }

            IEnumerable<CSteamID> MemberList()
            {
                var count = SteamMatchmaking.GetNumLobbyMembers(SessionId);
                for (var idx = 0; idx < count; ++idx)
                    yield return SteamMatchmaking.GetLobbyMemberByIndex(SessionId, idx);
            }
        }

        void OnMsgFail(SteamNetworkingMessagesSessionFailed_t t)
        {
            var playerId = t.m_info.m_identityRemote.GetSteamID64();
            var reason = ((ESteamNetConnectionEnd)t.m_info.m_eEndReason).Trim("k_ESteamNetConnectionEnd_"); ;
            feed.PlayerRemove?.Invoke(playerId, reason);
        }
    }

    private void ExitSession()
    {
        if (SessionId != CSteamID.Nil)
        {
            SteamMatchmaking.LeaveLobby(SessionId);
            SessionId = CSteamID.Nil;
        }

        SessionReady?.Dispose(); SessionReady = null;
        MemberUpdate?.Dispose(); MemberUpdate = null;
        DataUpdate?.Dispose(); DataUpdate = null;
        MsgInit?.Dispose(); MsgInit = null;
        MsgFail?.Dispose(); MsgFail = null;
    }

    #region Utils

    private string GetPingLocation()
    {
        var age = SteamNetworkingUtils.GetLocalPingLocation(out var loc);
        SteamNetworkingUtils.ConvertPingLocationToString(ref loc, out var str, int.MaxValue);
        return str;
    }

    private int GetPlayerPing(string str)
    {
        SteamNetworkingUtils.ParsePingLocationString(str, out var loc);
        return SteamNetworkingUtils.EstimatePingTimeFromLocalHost(ref loc);
    }

    private static ImageTexture LoadImage(int handle)
    {
        if (handle is 0) return null;
        if (!SteamUtils.GetImageSize(handle, out var width, out var height)) return null;

        var data = new byte[width * height * 4];
        if (!SteamUtils.GetImageRGBA(handle, data, data.Length)) return null;

        var image = Image.CreateFromData((int)width, (int)height, false, Image.Format.Rgba8, data);
        return ImageTexture.CreateFromImage(image);
    }

    #endregion
}
#endif
