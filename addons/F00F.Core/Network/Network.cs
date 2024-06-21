using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace F00F;

using TStatus = Status;

public partial class Network : Node
{
    private MultiplayerSpawner Spawn => field ??= GetNode<MultiplayerSpawner>("Spawn");

    #region Status

    public event Action StateChanged;

    public event Action<TStatus, string> ServerStatus;
    public event Action<TStatus, string> ClientStatus;

    public NetworkState State { get; set => this.Set(ref field, value, StateChanged); }

    public bool IsActive => IsServer || IsClient;

    public bool IsServer => State is
        NetworkState.StartingServer or
        NetworkState.ServerActive;

    public bool IsClient => State is
        NetworkState.ClientConnecting or
        NetworkState.ClientConnected;

    #endregion

    #region Constants

    public const int MinPort = 49152;
    public const int MaxPort = 65535;

    public const int ServerAuthority = 1;

    //public static bool DedicatedServer => OS.HasFeature("dedicated_server");
    //public static bool HeadlessServer => DisplayServer.GetName() is "headless";
    public static bool DedicatedServer { get; } = OS.HasFeature("dedicated_server") || DisplayServer.GetName() is "headless";

    public static int GamePort { get; } = (int)(MinPort + App.Hash % (MaxPort - MinPort));

    #endregion

    #region Initialise

    private Action<int> AddPlayer;
    private Action<int> RemovePlayer;
    private Action RemoveAllPlayers;
    public void Initialise<TPlayer>(
        Node SpawnTarget = null,
        Action<int, TPlayer> PlayerAdded = null,
        Action<int, TPlayer> PlayerRemoved = null) where TPlayer : Node
    {
        SpawnTarget ??= Spawn;

        var myPlayers = new Dictionary<int, TPlayer>();

        this.AddPlayer = AddPlayer;
        this.RemovePlayer = RemovePlayer;
        this.RemoveAllPlayers = RemoveAllPlayers;

        Spawn.Spawned += OnPlayerAdded;
        Spawn.Despawned += OnPlayerRemoved;
        Spawn.SpawnPath = Spawn.GetPathTo(SpawnTarget);
        Spawn.SpawnFunction = Callable.From<int, Node>(CreatePlayer);

        void AddPlayer(int pid)
        {
            var player = (TPlayer)Spawn.Spawn(pid);
            myPlayers.Add(pid, player);
            OnPlayerAdded(player);
        }

        void RemovePlayer(int pid)
        {
            if (myPlayers.Remove(pid, out var player))
                player.DetachChild(free: true);
            OnPlayerRemoved(player);
        }

        void RemoveAllPlayers()
        {
            while (myPlayers.Count > 0)
                RemovePlayer(myPlayers.Keys.First());
        }

        void OnPlayerAdded(Node player)
        {
            var pid = player.AuthId();
            Log.Debug($"Player added: {player.Name} ({LocalStr(pid)})");
            PlayerAdded?.Invoke(pid, (TPlayer)player);
        }

        void OnPlayerRemoved(Node player)
        {
            var pid = player.AuthId();
            Log.Debug($"Player removed: {player.Name} ({LocalStr(pid)})");
            PlayerRemoved?.Invoke(pid, (TPlayer)player);
        }

        Node CreatePlayer(int pid)
        {
            return Utils.New<TPlayer>(x =>
            {
                x.Name = $"{pid}";
                x.SetAuthId(pid);
            });
        }
    }

    #endregion

    #region Server/Client

    public bool StartServer() => StartServer(GamePort);
    public bool StartServer(string port) => StartServer(port.TryInt() ?? GamePort);

    public bool CreateClient(string connectStr = null)
    {
        connectStr.SplitConnectStr(out var address, out var port);
        return CreateClient(address ?? "localhost", port ?? GamePort);
    }

    public bool StartServer(int port)
    {
        Log.Debug($"Starting server on port {port}");
        State = NetworkState.StartingServer;
        ServerStatus?.Invoke(TStatus.Info, "Starting server...");

        var peer = new ENetMultiplayerPeer();
        var err = peer.CreateServer(port);
        if (CreateServerError()) return false;
        Multiplayer.MultiplayerPeer = peer;

        State = NetworkState.ServerActive;
        ServerStatus?.Invoke(TStatus.Success, "ACTIVE");

        if (!DedicatedServer)
        {
            var pid = this.PeerId();
            Log.Debug($"Peer connected: {pid} (host)");
            AddPlayer(pid);
        }

        return true;

        bool CreateServerError()
        {
            if (err is Error.Ok) return false;
            Log.Debug(Message());
            State = NetworkState.ServerError;
            ServerStatus?.Invoke(TStatus.Error, Message());
            return true;

            string Message() => $"Failed to start server on port {port}{err switch
            {
                Error.CantCreate => null,
                Error.AlreadyInUse => $" (port already in use)",
                _ => $" ({err})"
            }}";
        }
    }

    public bool CreateClient(string address, int port)
    {
        Log.Debug($"Connecting client to {address}:{port}");
        State = NetworkState.ClientConnecting;
        ClientStatus?.Invoke(TStatus.Info, "Connecting to server...");

        var peer = new ENetMultiplayerPeer();
        var err = peer.CreateClient(address, port);
        if (CreateClientError()) return false;
        Multiplayer.MultiplayerPeer = peer;

        return true;

        bool CreateClientError()
        {
            if (err is Error.Ok) return false;
            Log.Debug(Message());
            State = NetworkState.ClientError;
            ClientStatus?.Invoke(TStatus.Error, Message());
            return true;

            string Message() => $"Failed to connect to {address}:{port}{err switch
            {
                Error.CantCreate => null,
                Error.AlreadyInUse => $" (port already in use)",
                _ => $" ({err})"
            }}";
        }
    }

    public void StopServer()
    {
        ResetPeer();
        ServerStatus?.Invoke(TStatus.Info, "(not running)");
    }

    public void CloseClient()
    {
        ResetPeer();
        ClientStatus?.Invoke(TStatus.Info, "(not connected)");
    }

    private void ResetPeer()
    {
        RemoveAllPlayers();
        Multiplayer.MultiplayerPeer.Close();
        Multiplayer.MultiplayerPeer = new OfflineMultiplayerPeer();

        State = NetworkState.NoConnection;
    }

    #endregion

    #region Godot

    public sealed override void _EnterTree()
    {
        Multiplayer.ConnectedToServer += OnConnectedToServer;
        Multiplayer.ConnectionFailed += OnConnectionFailed;
        Multiplayer.ServerDisconnected += OnServerDisconnected;

        Multiplayer.PeerConnected += OnPeerConnected;
        Multiplayer.PeerDisconnected += OnPeerDisconnected;
    }

    public sealed override void _ExitTree()
    {
        Multiplayer.ConnectedToServer -= OnConnectedToServer;
        Multiplayer.ConnectionFailed -= OnConnectionFailed;
        Multiplayer.ServerDisconnected -= OnServerDisconnected;

        Multiplayer.PeerConnected -= OnPeerConnected;
        Multiplayer.PeerDisconnected -= OnPeerDisconnected;
    }

    #endregion

    #region Private

    private void OnConnectedToServer()
    {
        Log.Debug("Connected to server");
        State = NetworkState.ClientConnected;
        ClientStatus?.Invoke(TStatus.Success, "CONNECTED");
    }

    private void OnConnectionFailed()
    {
        ResetPeer();

        Log.Debug("Connection failed");
        State = NetworkState.ClientError;
        ClientStatus?.Invoke(TStatus.Error, "Failed to connect to server");
    }

    private void OnServerDisconnected()
    {
        ResetPeer();

        Log.Debug("Server disconnected");
        State = NetworkState.ClientDisconnected;
        ClientStatus?.Invoke(TStatus.Warn, "Lost Connection!");
    }

    private void OnPeerConnected(long pid)
    {
        Log.Debug($"Peer connected: {pid} ({LocalStr(pid)})");
        if (IsServer) AddPlayer((int)pid);
    }

    private void OnPeerDisconnected(long pid)
    {
        Log.Debug($"Peer disconnected: {pid} ({LocalStr(pid)})");
        if (IsServer) RemovePlayer((int)pid);
    }

    private string LocalStr(long pid)
        => pid == this.PeerId() ? "local" : "remote";

    #endregion
}
