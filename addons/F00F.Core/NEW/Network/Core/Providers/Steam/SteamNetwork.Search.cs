#if STEAM
using System;
using System.Collections.Generic;
using Steamworks;

namespace F00F.NEW;

using static Network;

internal partial class SteamNetwork
{
    public bool SessionSearch(SearchFilter filter, Action<SessionState[]> matches)
    {
        if (!Active) { Status.Warn.Push("Steam not active", Name); return false; }
        if (SessionId.IsValid()) { Status.Warn.Push("Already in session", Name); return false; }

        SetFilter();
        DoSearch();
        return true;

        void SetFilter()
        {
            if (filter.IsNull()) return;
            if (filter.SessionName.NotNullOrEmpty()) SteamMatchmaking.AddRequestLobbyListStringFilter(Tag.SessionName, filter.SessionName, ELobbyComparison.k_ELobbyComparisonEqual);
            if (filter.SessionOwner.NotNullOrEmpty()) SteamMatchmaking.AddRequestLobbyListStringFilter(Tag.PlayerName, filter.SessionOwner, ELobbyComparison.k_ELobbyComparisonEqual);
            if (filter.DistanceFilter.HasValue) SteamMatchmaking.AddRequestLobbyListDistanceFilter(Convert(filter.DistanceFilter.Value));
            if (filter.MaxSearchResults.HasValue) SteamMatchmaking.AddRequestLobbyListResultCountFilter(filter.MaxSearchResults.Value);
            if (filter.OpenOnly) SteamMatchmaking.AddRequestLobbyListFilterSlotsAvailable(1);
        }

        void DoSearch()
        {
            CallResult<LobbyMatchList_t>.Create(OnSessionSearch).Set(
                SteamMatchmaking.RequestLobbyList());

            void OnSessionSearch(LobbyMatchList_t t, bool fail)
            {
                if (fail)
                {
                    Status.Error.Push("Session search failed!", Name);
                    matches?.Invoke([]);
                    return;
                }

                matches?.Invoke([.. SessionList()]);

                IEnumerable<SessionState> SessionList()
                {
                    var count = t.m_nLobbiesMatching;
                    for (var idx = 0; idx < count; ++idx)
                    {
                        var sessionId = SteamMatchmaking.GetLobbyByIndex(idx);
                        var sessionName = SteamMatchmaking.GetLobbyData(sessionId, Tag.SessionName);
                        var memberCount = SteamMatchmaking.GetNumLobbyMembers(sessionId);
                        var memberLimit = SteamMatchmaking.GetLobbyMemberLimit(sessionId);
                        yield return new(sessionId.m_SteamID, sessionName, memberCount, memberLimit);
                    }
                }
            }
        }
    }

    #region Utils

    private static ELobbyDistanceFilter Convert(DistanceFilter x) => x switch
    {
        DistanceFilter.Far => ELobbyDistanceFilter.k_ELobbyDistanceFilterFar,
        DistanceFilter.Near => ELobbyDistanceFilter.k_ELobbyDistanceFilterClose,
        DistanceFilter.World => ELobbyDistanceFilter.k_ELobbyDistanceFilterWorldwide,
        _ => throw new NotImplementedException(),
    };

    #endregion
}
#endif
