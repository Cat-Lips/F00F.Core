using System;

namespace F00F.NEW;

internal interface INetworkProvider : INetworkCommon, INetworkController
{
    int MaxSupportedPlayers => 0;
    string DefaultPlayerName => Environment.UserName;

    bool HostSession(Network.SessionHost host, Network.SessionFeed feed) => false;
    bool JoinSession(Network.SessionJoin join, Network.SessionFeed feed) => false;
    bool SessionSearch(Network.SearchFilter filter, Action<Network.SessionState[]> matches) => false;

    void ExitSession() { }
}
