#if STEAM
using System;
using Steamworks;

namespace F00F.NEW;

internal partial class SteamNetwork
{
    #region Send

    public bool Send(ulong playerId, ReadOnlySpan<byte> msg, int channel, bool reliable = false, bool ordered = false)
        => Send(playerId, msg, Flags(reliable, ordered), channel);

    public bool Send(ulong[] playerIds, ReadOnlySpan<byte> msg, int channel, bool reliable = false, bool ordered = false)
        => Send(playerIds, msg, Flags(reliable, ordered), channel);

    private bool Send(ulong playerId, ReadOnlySpan<byte> msg, int flags, int channel)
    {
        fixed (byte* ptr = msg)
        {
            var target = NetworkId(playerId);
            var err = SteamNetworkingMessages.SendMessageToUser(ref target, (nint)ptr, (uint)msg.Length, flags, channel);
            if (err is EResult.k_EResultOK) return true;
            Status.Warn.Push(err.Trim("k_EResult"), Name);
            return false;
        }
    }

    private bool Send(ulong[] playerIds, ReadOnlySpan<byte> msg, int flags, int channel)
    {
        var ok = true;
        fixed (byte* ptr = msg)
        {
            foreach (var playerId in playerIds)
            {
                var target = NetworkId(playerId);
                var err = SteamNetworkingMessages.SendMessageToUser(ref target, (nint)ptr, (uint)msg.Length, flags, channel);
                if (err is not EResult.k_EResultOK) { Status.Warn.Push(err.Trim("k_EResult"), Name); ok = false; }
            }
        }
        return ok;
    }

    #endregion

    #region Recv

    public int ChannelCount { get; set => field = value.ClampMin(1); } = 1;

    private const int RecvBufferSize = 32;
    private readonly nint[] RecvBuffer = new nint[RecvBufferSize];
    public void Recv(Action<ulong, ReadOnlySpan<byte>, int> OnRecv)
    {
        for (var channel = 0; channel < ChannelCount; ++channel)
        {
            while (true)
            {
                var msgCount = SteamNetworkingMessages.ReceiveMessagesOnChannel(channel, RecvBuffer, RecvBufferSize);
                if (msgCount.IsZero()) break;
                for (var i = 0; i < msgCount; ++i)
                {
                    var ptr = RecvBuffer[i];
                    var recv = SteamNetworkingMessage_t.FromIntPtr(ptr);
                    var span = recv.m_pData.Span(recv.m_cbSize);
                    var source = recv.m_identityPeer.GetSteamID64();
                    try { OnRecv?.Invoke(source, span, channel); }
                    catch (Exception ex) { Status.Warn.Push($"{ex.Message} [Channel {channel}]", Name); }
                    SteamNetworkingMessage_t.Release(ptr);
                }
            }
        }
    }

    #endregion

    #region Utils

    private static SteamNetworkingIdentity NetworkId(ulong playerId)
    {
        var networkId = new SteamNetworkingIdentity();
        networkId.SetSteamID64(playerId);
        return networkId;
    }

    private int Flags(bool reliable, bool ordered)
    {
        return reliable ? Constants.k_nSteamNetworkingSend_Reliable
             : ordered ? Constants.k_nSteamNetworkingSend_Unreliable : Constants.k_nSteamNetworkingSend_UnreliableNoNagle;
    }

    #endregion
}
#endif
