#if STEAM
using System;
using System.Collections.Generic;
using Steamworks;

namespace F00F.NEW;

internal partial class SteamNetwork
{
    public bool SessionSearch(Network.SearchFilter filter, Action<Network.SessionState[]> matches)
    {
        if (!Active) { Status.Warn.Push("Steam not active", Name); return false; }
        if (SessionId != CSteamID.Nil) { Status.Warn.Push("Already in session", Name); return false; }

        SetFilter();
        DoSearch();
        return true;

        void SetFilter()
        {
            if (filter.IsNull()) return;
            if (filter.SessionHost.NotNullOrEmpty()) SteamMatchmaking.AddRequestLobbyListStringFilter(Tag.SessionHost, filter.SessionHost, ELobbyComparison.k_ELobbyComparisonEqual);
            if (filter.SessionName.NotNullOrEmpty()) SteamMatchmaking.AddRequestLobbyListStringFilter(Tag.SessionName, filter.SessionName, ELobbyComparison.k_ELobbyComparisonEqual);
            if (filter.DistanceFilter.HasValue) SteamMatchmaking.AddRequestLobbyListDistanceFilter(Convert(filter.DistanceFilter.Value));
            if (filter.MaxSearchResults.HasValue) SteamMatchmaking.AddRequestLobbyListResultCountFilter(filter.MaxSearchResults.Value);
            if (filter.OpenOnly) SteamMatchmaking.AddRequestLobbyListFilterSlotsAvailable(1);
        }

        void DoSearch()
        {
            SteamMatchmaking.AddRequestLobbyListStringFilter(Tag.GameName, App.Name, ELobbyComparison.k_ELobbyComparisonEqual);
            CallResult<LobbyMatchList_t>.Create(OnSessionSearch).Set(SteamMatchmaking.RequestLobbyList());

            void OnSessionSearch(LobbyMatchList_t t, bool fail)
            {
                if (fail)
                {
                    Status.Error.Push("Session search failed!", Name);
                    matches?.Invoke([]);
                    return;
                }

                matches?.Invoke([.. SessionList()]);

                IEnumerable<Network.SessionState> SessionList()
                {
                    var count = t.m_nLobbiesMatching;
                    for (var idx = 0; idx < count; ++idx)
                    {
                        var sessionId = SteamMatchmaking.GetLobbyByIndex(idx);
                        var sessionHost = SteamMatchmaking.GetLobbyData(sessionId, Tag.SessionHost);
                        var sessionName = SteamMatchmaking.GetLobbyData(sessionId, Tag.SessionName);
                        var sessionPing = GetPlayerPing(SteamMatchmaking.GetLobbyData(sessionId, Tag.PingLocation));
                        var sessionLocn = SteamMatchmaking.GetLobbyData(sessionId, Tag.SessionLocation);
                        var memberCount = SteamMatchmaking.GetNumLobbyMembers(sessionId);
                        var memberLimit = SteamMatchmaking.GetLobbyMemberLimit(sessionId);

                        yield return new(sessionId.m_SteamID, sessionHost, sessionName, sessionLocn, sessionPing, memberCount, memberLimit);
                    }
                }
            }
        }
    }

    #region Utils

    private static ELobbyDistanceFilter Convert(Network.DistanceFilter x) => x switch
    {
        Network.DistanceFilter.Far => ELobbyDistanceFilter.k_ELobbyDistanceFilterFar,
        Network.DistanceFilter.Near => ELobbyDistanceFilter.k_ELobbyDistanceFilterClose,
        Network.DistanceFilter.World => ELobbyDistanceFilter.k_ELobbyDistanceFilterWorldwide,
        _ => throw new NotImplementedException(),
    };

    #endregion
}
#endif
