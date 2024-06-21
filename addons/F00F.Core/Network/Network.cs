using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace F00F;

using TStatus = Status;

public partial class Network : Node
{
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

    public void Initialise(
        Node SpawnTarget,
        Func<Node> CreatePlayer,
        Action<int, Node> OnPlayerAdded = null,
        Action<int, Node> OnPlayerRemoved = null)
    {
        var myPlayers = new Dictionary<int, Node>();
        var mySpawner = NewSpawner("PlayerSpawn", SpawnTarget);

        this.AddPlayer = AddPlayer;
        this.RemovePlayer = RemovePlayer;
        this.RemoveAllPlayers = RemoveAllPlayers;

        mySpawner.Spawned += OnPlayerSpawned;
        mySpawner.Despawned += OnPlayerDespawned;
        mySpawner.SpawnFunction = Callable.From<int, Node>(SpawnPlayer);

        Node SpawnPlayer(int pid)
        {
            var player = CreatePlayer();
            player.Name = $"{pid}";
            player.SetAuthId(pid);
            return player;
        }

        void OnPlayerSpawned(Node player)
        {
            var pid = player.AuthId();
            Log.Debug($"Player added: {player.Name} ({LocalStr(pid)})");
            OnPlayerAdded?.Invoke(pid, player);
        }

        void OnPlayerDespawned(Node player)
        {
            var pid = player.AuthId();
            Log.Debug($"Player removed: {player.Name} ({LocalStr(pid)})");
            OnPlayerRemoved?.Invoke(pid, player);
        }

        void AddPlayer(int pid)
        {
            var player = mySpawner.Spawn(pid);
            myPlayers.Add(pid, player);
            OnPlayerSpawned(player);
        }

        void RemovePlayer(int pid)
        {
            if (myPlayers.Remove(pid, out var player))
                player.DetachChild(free: true);
            OnPlayerDespawned(player);
        }

        void RemoveAllPlayers()
        {
            while (myPlayers.Count > 0)
                RemovePlayer(myPlayers.Keys.First());
        }
    }

    //public void Initialise(
    //    Func<Node> CreatePlayer,
    //    Action<int, Node> OnPlayerAdded = null,
    //    Action<int, Node> OnPlayerRemoved = null)
    //    => Initialise(null, CreatePlayer, OnPlayerAdded, OnPlayerRemoved);

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
            AddPlayer?.Invoke(pid);
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
        RemoveAllPlayers?.Invoke();
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
        if (IsServer) AddPlayer?.Invoke((int)pid);
    }

    private void OnPeerDisconnected(long pid)
    {
        Log.Debug($"Peer disconnected: {pid} ({LocalStr(pid)})");
        if (IsServer) RemovePlayer?.Invoke((int)pid);
    }

    private string LocalStr(long pid)
        => pid == this.PeerId() ? "local" : "remote";

    private MultiplayerSpawner NewSpawner(string name, Node target)
    {
        var x = new MultiplayerSpawner { Name = name };
        AddChild(x);
        x.SpawnPath = x.GetPathTo(target ?? x);
        return x;
    }

    #endregion
}
