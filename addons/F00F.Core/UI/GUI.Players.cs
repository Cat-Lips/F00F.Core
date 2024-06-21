using Godot;
using static Godot.MultiplayerApi;

namespace F00F;

public partial class GUI
{
    public void AddPlayer(int pid, out IPlayerData data)
    {
        if (this.IsLocal(pid))
        {
            var localPlayerData = (PlayerData)PlayerData;
            PlayerList.AddPlayer(pid, localPlayerData);
            data = localPlayerData;

            PlayerData.NameChanged += x => Rpc(MethodName.SetRemotePlayerName, pid, x);
            PlayerData.ColorChanged += x => Rpc(MethodName.SetRemotePlayerColor, pid, x);
            PlayerData.AvatarChanged += x => Rpc(MethodName.SetRemotePlayerAvatar, pid, x);
            PlayerData.StateChanged += (key, value) => Rpc(MethodName.SetRemotePlayerState, pid, key, value);
            PlayerData.ScoreChanged += x => Rpc(MethodName.SetRemotePlayerScore, pid, x);
        }
        else
        {
            var remotePlayerData = new PlayerData();
            PlayerList.AddPlayer(pid, remotePlayerData);
            data = remotePlayerData;

            RpcId(pid, MethodName.GetRemotePlayerData, this.PeerId());
        }
    }

    public void RemovePlayer(int pid)
        => PlayerList.RemovePlayer(pid);

    #region RPC

    [Rpc(RpcMode.AnyPeer, TransferChannel = Const.TransferChannel.PlayerData)]
    private void SetRemotePlayerName(int pid, string name)
        => PlayerList[pid].PlayerName = name;

    [Rpc(RpcMode.AnyPeer, TransferChannel = Const.TransferChannel.PlayerData)]
    private void SetRemotePlayerColor(int pid, /*in */Color color)
        => PlayerList[pid].PlayerColor = color;

    [Rpc(RpcMode.AnyPeer, TransferChannel = Const.TransferChannel.PlayerData)]
    private void SetRemotePlayerAvatar(int pid, string avatar)
        => PlayerList[pid].PlayerAvatar = avatar;

    [Rpc(RpcMode.AnyPeer, TransferChannel = Const.TransferChannel.PlayerData)]
    private void SetRemotePlayerState(int pid, string key, string value)
        => PlayerList[pid][key] = value;

    [Rpc(RpcMode.AnyPeer, TransferChannel = Const.TransferChannel.PlayerData)]
    private void SetRemotePlayerScore(int pid, float score)
        => PlayerList[pid].CurrentScore = score;

    [Rpc(RpcMode.AnyPeer, TransferChannel = Const.TransferChannel.PlayerData)]
    private void GetRemotePlayerData(int pid)
    {
        var peer = this.PeerId();
        RpcId(pid, MethodName.SetRemotePlayerName, peer, PlayerData.PlayerName);
        RpcId(pid, MethodName.SetRemotePlayerColor, peer, PlayerData.PlayerColor);
        RpcId(pid, MethodName.SetRemotePlayerAvatar, peer, PlayerData.PlayerAvatar);
        PlayerData.State.ForEach(x => RpcId(pid, MethodName.SetRemotePlayerState, peer, x, PlayerData[x]));
        RpcId(pid, MethodName.SetRemotePlayerScore, peer, PlayerData.CurrentScore);
    }

    #endregion
}
