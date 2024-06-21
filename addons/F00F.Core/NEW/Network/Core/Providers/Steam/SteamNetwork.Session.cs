#if STEAM
using Godot;
using Steamworks;

namespace F00F.NEW;

internal partial class SteamNetwork
{
    #region Tags

    private static class Tag
    {
        public const string PlayerName = "PN";
        public const string SessionName = "SN";
        public const string PlayerLocation = "PL";
    }

    #endregion

    #region Private

    private CSteamID SessionId;

    private Callback<LobbyEnter_t> SessionReady;
    private Callback<LobbyDataUpdate_t> DataUpdate;
    private Callback<LobbyChatUpdate_t> MemberUpdate;

    #endregion

    private void InitSession(string PlayerName, Network.SessionFeed feed)
    {
        SessionReady = Callback<LobbyEnter_t>.Create(OnSessionReady);
        DataUpdate = Callback<LobbyDataUpdate_t>.Create(OnDataUpdate);
        MemberUpdate = Callback<LobbyChatUpdate_t>.Create(OnMemberUpdate);

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
            SteamMatchmaking.SetLobbyMemberData(SessionId, Tag.PlayerLocation, SteamUtils.GetIPCountry());
            // Any other steam data (like GetIPCountry) that we could auto included here?
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
                var SessionName = SteamMatchmaking.GetLobbyData(SessionId, Tag.SessionName);
                var MemberCount = SteamMatchmaking.GetNumLobbyMembers(SessionId);
                var MemberLimit = SteamMatchmaking.GetLobbyMemberLimit(SessionId);
                feed.SessionUpdate?.Invoke(new(SessionId.m_SteamID, SessionName, MemberCount, MemberLimit));
            }

            void DoPlayerUpdate()
            {
                var PlayerName = SteamMatchmaking.GetLobbyMemberData(sessionId, playerId, Tag.PlayerName);
                var PlayerAvatar = LoadImage(SteamFriends.GetSmallFriendAvatar(playerId));
                var PlayerLocation = SteamMatchmaking.GetLobbyMemberData(sessionId, playerId, Tag.PlayerLocation);
                feed.PlayerUpdate?.Invoke(new(playerId.m_SteamID, PlayerName, PlayerAvatar, PlayerLocation));
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
    }

    private void ExitSession()
    {
        if (SessionId.IsValid())
            SteamMatchmaking.LeaveLobby(SessionId);
        SessionReady?.Dispose(); SessionReady = null;
        MemberUpdate?.Dispose(); MemberUpdate = null;
        DataUpdate?.Dispose(); DataUpdate = null;
        SessionId = CSteamID.Nil;
    }

    #region Utils

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
