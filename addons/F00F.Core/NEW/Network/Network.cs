using System;
using Godot;

namespace F00F.NEW;

[Tool]
public partial class Network : Node
{
    #region Consts

    public const int MinPort = 49152;
    public const int MaxPort = 65535;

    public static int GamePort { get; } = (int)(MinPort + App.Hash % (MaxPort - MinPort));

    public static bool IsDedicatedServer { get; } = OS.HasFeature("dedicated_server") || DisplayServer.GetName() is "headless";

    public const int ServerAuthority = 1;

    #endregion

    #region State

    public event Action StateChanged;

    public event Action<Status, string> ServerStatus;
    public event Action<Status, string> ClientStatus;

    public NetworkState State { get; set => this.Set(ref field, value, StateChanged); }

    public bool IsActive => IsServer || IsClient;
    public bool NotActive => !IsActive;

    public bool IsServer => State is
        NetworkState.StartingServer or
        NetworkState.ServerActive;

    public bool IsClient => State is
        NetworkState.ClientConnecting or
        NetworkState.ClientConnected;

    public bool Success => State is
        NetworkState.ServerActive or
        NetworkState.ClientConnected;

    public bool Error => State is
        NetworkState.ServerError or
        NetworkState.ClientError;

    public bool Warn => State is
        NetworkState.ClientDisconnected;

    public bool IsReset => State is NetworkState.NoConnection;
    public bool IsReady => State is NetworkState.NoConnection;
    public bool HasState => State is not NetworkState.NoConnection;

    public void Reset() => ExitSession();

    #region Private

    private void SERVER(NetworkState state, Status status, string msg)
    {
        State = state;
        ServerStatus?.Invoke(status, msg);
    }

    private void CLIENT(NetworkState state, Status status, string msg)
    {
        State = state;
        ClientStatus?.Invoke(status, msg);
    }

    #endregion

    #endregion

    #region Init

    public static Network Instance { get; private set; }

    public Network()
    {
        Instance = this;
        ProcessMode = ProcessModeEnum.Always;
    }

    public sealed override void _Ready()
    {
        InitServices();
        ParseCmdLine(this);
        ParseDebugCmdLine(this);
    }

    #endregion

    #region API

    #region Data

    public enum SessionScope { Public, Friends, InviteOnly }
    public enum DistanceFilter { Near, Far, World }

    public record SessionState(ulong SessionId, string SessionName, int MemberCount, int MemberLimit);
    public record PlayerProfile(ulong PlayerId, string PlayerName, Texture2D PlayerAvatar, string PlayerLocation);
    public record SearchFilter(string PlayerName = null, string SessionName = null, DistanceFilter? Distance = null, int? MaxResults = null, bool OpenOnly = true);

    public record SessionJoin(string PlayerName, ulong SessionId);
    public record SessionHost(string PlayerName, string SessionName, string SessionDesc, SessionScope SessionScope, int MaxPlayers);
    public record SessionFeed(Action<ulong> PlayerAdded = null, Action<ulong, string> PlayerRemoved = null, Action<PlayerProfile> PlayerUpdated = null, Action<SessionState> SessionUpdated = null);

    public SessionFeed Feed { get; private set; }

    #endregion

    #region Server

    public bool StartServer(SessionHost host, SessionFeed feed = null)
    {
        if (IsClient) { Status.Warn.Push("StartServer: Active Client!!!"); return false; }
        if (IsServer) { Status.Warn.Push("StartServer: Server already started"); return false; }

        SERVER(NetworkState.StartingServer, Status.Info, "Starting server...");
        var result = CoreProvider.HostSession(host, Feed = feed ??= new());
        SERVER(NetworkState.ServerError, Status.Error, "Session failed");
        return result;
    }

    public bool StopServer()
    {
        if (IsClient) { Status.Warn.Push("StopServer: Active Client!!!"); return false; }
        if (!IsServer) { Status.Warn.Push("StopServer: Server not started"); return false; }

        CoreProvider.ExitSession();
        SERVER(NetworkState.NoConnection, Status.Info, "(not running)");
        return true;
    }

    #endregion

    #region Client

    public bool ConnectClient(SessionJoin join, SessionFeed feed = null)
    {
        if (IsServer) { Status.Warn.Push("ConnectClient: Active Server!!!"); return false; }
        if (IsClient) { Status.Warn.Push("ConnectClient: Client already connected"); return false; }

        CLIENT(NetworkState.ClientConnecting, Status.Info, "Connecting to server...");
        var result = CoreProvider.JoinSession(join, Feed = feed ??= new());
        CLIENT(NetworkState.ClientError, Status.Error, "Session failed");
        return result;
    }

    public bool CloseClient()
    {
        if (IsServer) { Status.Warn.Push("CloseClient: Active Server!!!"); return false; }
        if (!IsClient) { Status.Warn.Push("CloseClient: Client not connected"); return false; }

        CoreProvider.ExitSession();
        CLIENT(NetworkState.NoConnection, Status.Info, "(not connected)");
        return true;
    }

    #endregion

    #region Search

    // TODO

    #endregion

    #region Xtras

    public void ExitSession()
    {
        if (IsServer) StopServer();
        if (IsClient) CloseClient();
        else State = NetworkState.NoConnection;
    }

    #endregion

    #endregion
}
