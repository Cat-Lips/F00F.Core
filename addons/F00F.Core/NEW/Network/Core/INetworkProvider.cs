using System;

namespace F00F.NEW;

using static Network;

internal interface INetworkProvider : INetworkCommon
{
    int MaxSupportedPlayers => 0;
    string DefaultPlayerName => Environment.UserName;

    bool HostSession(SessionHost host, SessionFeed feed) => false;
    bool JoinSession(SessionJoin join, SessionFeed feed) => false;
    bool SessionSearch(SearchFilter filter, Action<SessionState[]> matches) => false;

    void ExitSession() { }
}
