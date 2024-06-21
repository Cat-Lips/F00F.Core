using System;

namespace F00F.NEW;

internal interface INetworkController
{
    int LocalPlayerId => 0;
    int ChannelCount { get => 0; set { } }

    bool AllowJoin { get => false; set { } }
    event Action AllowJoinChanged { add { } remove { } }

    void Recv(Action<ulong, ReadOnlySpan<byte>, int> OnRecv) { }
    bool Send(ulong playerId, ReadOnlySpan<byte> msg, int channel, bool reliable, bool ordered) => false;
    bool Send(ulong[] playerId, ReadOnlySpan<byte> msg, int channel, bool reliable, bool ordered) => false;
}
