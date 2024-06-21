using Godot;
using static Godot.MultiplayerApi;

namespace F00F;

[Tool]
public partial class GUI : CanvasLayer
{
    private readonly PlayerData playerData = new();

    public MainMenu MainMenu => field ??= GetNode<MainMenu>("MainMenu");
    public QuickHelp QuickHelp => field ??= GetNode<QuickHelp>("QuickHelp");
    public PlayerList PlayerList => field ??= GetNode<PlayerList>("PlayerList");

    public IPlayerData PlayerData => playerData;

    public GUI()
        => Layer = Const.CanvasLayer.GUI;

    public void AddPlayer(int pid)
    {
        if (this.IsLocal(pid))
        {
            PlayerList.AddPlayer(pid, playerData);

            PlayerData.NameChanged += x => Rpc(MethodName.SetRemotePlayerName, pid, x);
            PlayerData.ColorChanged += x => Rpc(MethodName.SetRemotePlayerColor, pid, x);
            PlayerData.AvatarChanged += x => Rpc(MethodName.SetRemotePlayerAvatar, pid, x);
            PlayerData.StateChanged += (key, value) => Rpc(MethodName.SetRemotePlayerState, pid, key, value);
            PlayerData.ScoreChanged += x => Rpc(MethodName.SetRemotePlayerScore, pid, x);
        }
        else
        {
            PlayerList.AddPlayer(pid, new());
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
