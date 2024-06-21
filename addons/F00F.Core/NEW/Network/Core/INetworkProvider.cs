using System;

namespace F00F.NEW;

using static Network;

internal interface INetworkProvider : INetworkCommon
{
    int MaxSupportedPlayers => 0;
    string DefaultPlayerName => Environment.UserName;

    bool HostSession(SessionHost host, SessionFeed feed) => false;
    bool JoinSession(SessionJoin join, SessionFeed feed) => false;

    void ExitSession() { }

    //void SessionSearch(Action<SessionState[]> feed, Action error) => SessionSearch(null, feed, error);
    //void SessionSearch(SearchFilter filter, Action<SessionState[]> feed, Action error) => throw new NotImplementedException();
}
