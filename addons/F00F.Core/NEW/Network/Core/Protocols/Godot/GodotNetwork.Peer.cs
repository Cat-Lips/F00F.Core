using System;
using System.Collections.Generic;
using Godot;

namespace F00F.NEW;

internal partial class GodotPeerExtension : MultiplayerPeerExtension
{
    #region Private

    private record MsgData(int SenderId, byte[] Message, int Channel);
    private readonly Queue<MsgData> msgQueue = [];

    #endregion

    public INetworkController Controller { get; set => this.Set(ref field, value, OnControllerSet); }
    private void OnControllerSet(INetworkController old, INetworkController @new)
    {
        InitProvider();

        void InitProvider()
        {
            OnCanJoinChanged();
            old?.AllowJoinChanged -= OnCanJoinChanged;
            @new?.AllowJoinChanged += OnCanJoinChanged;

            void OnCanJoinChanged()
                => RefuseNewConnections = Controller.NotNull() && !Controller.AllowJoin;
        }
    }

    #region Godot

    public sealed override void _Close() => Network.Instance.ExitSession();
    public sealed override void _DisconnectPeer(int peer, bool force)
        => throw new NotImplementedException(); // TODO: Add to interface

    public sealed override bool _IsServer() => Network.Instance.IsServer;
    public sealed override bool _IsServerRelaySupported() => true; // Always

    public sealed override int _GetPacketPeer() => msgQueue.Peek().SenderId;
    public sealed override int _GetPacketChannel() => msgQueue.Peek().Channel;
    public sealed override int _GetAvailablePacketCount() => msgQueue.Count;
    //public sealed override TransferModeEnum _GetPacketMode() => -1; // Unused/Unavailable
    public sealed override ConnectionStatus _GetConnectionStatus() => Convert(Network.Instance.State);

    public sealed override int _GetUniqueId() => Controller.LocalPlayerId;
    public sealed override int _GetMaxPacketSize() => int.MaxValue; // TODO: Add to interface

    public sealed override void _SetRefuseNewConnections(bool enable) => Controller.AllowJoin = !enable;
    public sealed override bool _IsRefusingNewConnections() => !Controller.AllowJoin;

    public sealed override int _GetTransferChannel() => TransferChannel;
    public sealed override TransferModeEnum _GetTransferMode() => TransferMode;

    private int TargetPeer { get; set; }
    public sealed override void _SetTargetPeer(int peer) => TargetPeer = peer;
    public sealed override void _SetTransferChannel(int channel) => TransferChannel = channel;
    public sealed override void _SetTransferMode(TransferModeEnum mode) => TransferMode = mode;

    #endregion

    #region Send

    public sealed override Error _PutPacketScript(byte[] buffer)
    {
        return Send() ? Error.Ok : Error.Failed;

        bool Send()
        {
            return TransferMode switch
            {
                TransferModeEnum.Reliable => Send(reliable: true, ordered: true),
                TransferModeEnum.Unreliable => Send(reliable: false, ordered: false),
                TransferModeEnum.UnreliableOrdered => Send(reliable: false, ordered: true),
                _ => throw new NotImplementedException(),
            };

            bool Send(bool reliable, bool ordered)
                => Controller.Send((ulong)TargetPeer, buffer.AsSpan(), TransferChannel, reliable, ordered);
        }
    }

    #endregion

    #region Recv

    public sealed override void _Poll()
    {
        Controller.Recv(OnRecv);

        void OnRecv(ulong senderId, ReadOnlySpan<byte> msg, int channel)
            => msgQueue.Enqueue(new((int)senderId, msg.ToArray(), channel));
    }

    public sealed override byte[] _GetPacketScript()
        => msgQueue.Dequeue().Message;

    #endregion

    #region Utils

    private static ConnectionStatus Convert(NetworkState scope) => scope switch
    {
        NetworkState.StartingServer => ConnectionStatus.Connecting,
        NetworkState.ClientConnecting => ConnectionStatus.Connecting,

        NetworkState.ServerActive => ConnectionStatus.Connected,
        NetworkState.ClientConnected => ConnectionStatus.Connected,

        NetworkState.ServerError => ConnectionStatus.Disconnected,
        NetworkState.ClientError => ConnectionStatus.Disconnected,
        NetworkState.NoConnection => ConnectionStatus.Disconnected,
        NetworkState.ClientDisconnected => ConnectionStatus.Disconnected,
        _ => throw new NotImplementedException(),
    };

    #endregion
}
