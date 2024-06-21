using System;
using System.Collections.Generic;
using Godot;

namespace F00F.NEW;

using ActivePlayers = IReadOnlyDictionary<ulong, Network.PlayerProfile>;
using PlayerLookup = Dictionary<ulong, Network.PlayerProfile>;

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
    public event Action<Status, string> CombinedStatus;

    public NetworkState State { get; private set => this.Set(ref field, value, StateChanged); }

    public bool IsServer => State is
        NetworkState.StartingServer or
        NetworkState.ServerActive or
        NetworkState.ServerError;

    public bool IsClient => State is
        NetworkState.ClientDisconnected or
        NetworkState.ClientConnecting or
        NetworkState.ClientConnected or
        NetworkState.ClientError;

    public bool IsActive => State is
        NetworkState.ClientConnecting or
        NetworkState.ClientConnected or
        NetworkState.StartingServer or
        NetworkState.ServerActive;
    public bool NotActive => !IsActive;

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
    public bool HasPlayers => CurrentPlayers?.Count is not 0;

    public string SessionType(string none = "") =>
        IsServer ? "SERVER" :
        IsClient ? "CLIENT" : none;

    public string SessionTitle(string dflt)
        => SessionTitle(CurrentSession, dflt);

    public Status? SessionStatus() =>
        Success ? F00F.Status.Success :
        Error ? F00F.Status.Error :
        Warn ? F00F.Status.Warn : null;

    #region Private

    private static string SessionTitle(SessionState ss, string dflt)
        => ss is null ? dflt : $"{ss.SessionName ?? dflt} - {ss.MemberCount}/{ss.MemberLimit}";

    private void SERVER(NetworkState state, Status status, string msg)
    {
        State = state;
        ServerStatus?.Invoke(status, msg);
        CombinedStatus?.Invoke(status, msg);
    }

    private void CLIENT(NetworkState state, Status status, string msg)
    {
        State = state;
        ClientStatus?.Invoke(status, msg);
        CombinedStatus?.Invoke(status, msg);
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
    public record SearchFilter(string SessionName = null, string SessionOwner = null, DistanceFilter? DistanceFilter = null, int? MaxSearchResults = null, bool OpenOnly = true);

    public record SessionJoin(string PlayerName, ulong SessionId);
    public record SessionHost(string PlayerName, string SessionName, string SessionDesc, SessionScope SessionScope, int MaxPlayers);
    internal record SessionFeed(Action<ulong> PlayerAdd, Action<PlayerProfile> PlayerUpdate, Action<ulong, string> PlayerRemove, Action<SessionState> SessionUpdate);

    #endregion

    #region State

    public event Action SessionReset;
    public event Action SessionUpdate;

    public event Action PlayersReset;
    public event Action<ulong> PlayerInsert;
    public event Action<PlayerProfile> PlayerUpdate;
    public event Action<ulong, string> PlayerRemove;

    public SessionState CurrentSession { get; private set => this.Set(ref field, value, OnSessionSet); }
    public ActivePlayers CurrentPlayers { get; private set => this.Set(ref field, value, OnPlayersSet); }

    #region Private

    private static void InitSession(Network self, out SessionFeed feed)
    {
        var lookup = new PlayerLookup();
        self.CurrentPlayers = lookup;
        feed = new SessionFeed(OnPlayerAdd, OnPlayerUpdate, OnPlayerRemove, OnSessionUpdate);

        void OnPlayerAdd(ulong id)
        {
            lookup.Add(id, null);
            self.PlayerInsert?.Invoke(id);
        }

        void OnPlayerUpdate(PlayerProfile pp)
        {
            if (!lookup.ContainsKey(pp.PlayerId))
                self.PlayerInsert?.Invoke(pp.PlayerId);

            lookup[pp.PlayerId] = pp;
            self.PlayerUpdate?.Invoke(pp);
        }

        void OnPlayerRemove(ulong id, string reason)
        {
            lookup.Remove(id);
            self.PlayerRemove?.Invoke(id, reason);
        }

        void OnSessionUpdate(SessionState ss)
            => self.CurrentSession = ss;
    }

    private static void DnitSession(Network self)
    {
        self.CurrentSession = null;
        self.CurrentPlayers = null;
    }

    private void OnSessionSet()
    {
        if (CurrentSession.IsNull())
            SessionReset?.Invoke();
        else SessionUpdate?.Invoke();
    }

    private void OnPlayersSet()
    {
        if (CurrentPlayers.IsNull())
            PlayersReset?.Invoke();
    }

    #endregion

    #endregion

    #region Server

    public bool StartServer(SessionHost host)
    {
        if (IsClient) { Status.Warn.Push("StartServer: Active Client!!!", Name); return false; }
        if (IsServer) { Status.Warn.Push("StartServer: Server already started", Name); return false; }

        InitSession(this, out var feed);
        SERVER(NetworkState.StartingServer, Status.Info, "Starting server...");
        var success = CoreProvider.HostSession(host, feed);
        if (!success) SERVER(NetworkState.ServerError, Status.Error, "Session failed");
        if (!success) DnitSession(this);
        return success;
    }

    public bool StopServer()
    {
        if (IsClient) { Status.Warn.Push("StopServer: Active Client!!!", Name); return false; }
        if (!IsServer) { Status.Warn.Push("StopServer: Server not started", Name); return false; }

        CoreProvider.ExitSession();
        SERVER(NetworkState.NoConnection, Status.Info, "(not running)");
        DnitSession(this);
        return true;
    }

    #endregion

    #region Client

    public bool ConnectClient(SessionJoin join)
    {
        if (IsServer) { Status.Warn.Push("ConnectClient: Active Server!!!", Name); return false; }
        if (IsClient) { Status.Warn.Push("ConnectClient: Client already connected", Name); return false; }

        InitSession(this, out var feed);
        CLIENT(NetworkState.ClientConnecting, Status.Info, "Connecting to server...");
        var success = CoreProvider.JoinSession(join, feed);
        if (!success) CLIENT(NetworkState.ClientError, Status.Error, "Session failed");
        if (!success) DnitSession(this);
        return success;
    }

    public bool CloseClient()
    {
        if (IsServer) { Status.Warn.Push("CloseClient: Active Server!!!", Name); return false; }
        if (!IsClient) { Status.Warn.Push("CloseClient: Client not connected", Name); return false; }

        CoreProvider.ExitSession();
        CLIENT(NetworkState.NoConnection, Status.Info, "(not connected)");
        DnitSession(this);
        return true;
    }

    #endregion

    #region Search

    public bool Search(SearchFilter filter, Action<SessionState[]> matches)
    {
        if (IsServer) { Status.Warn.Push("Search: Active Server!!!", Name); return false; }
        if (IsClient) { Status.Warn.Push("Search: Active Client!!!", Name); return false; }

        var success = CoreProvider.SessionSearch(filter, matches);
        if (!success) Status.Error.Push("Search failed", Name);
        return success;
    }

    #endregion

    #region Xtras

    public void ExitSession()
    {
        if (IsServer) StopServer();
        if (IsClient) CloseClient();
        else State = NetworkState.NoConnection;
    }

    public void Reset()
        => ExitSession();

    #endregion

    #endregion
}
