using System;
using System.Collections.Generic;
using System.Linq;

namespace F00F.NEW;

public partial class Network
{
    #region Private

    private IDictionary<string, INetworkProtocol> MyProtocols => field ??= InitLookup<INetworkProtocol>("Protocols");
    private IDictionary<string, INetworkProvider> MyProviders => field ??= InitLookup<INetworkProvider>("Providers");

    private INetworkProtocol MyProtocol { get; set => this.Set(ref field, value, InitProtocol); }
    private INetworkProvider MyProvider { get; set => this.Set(ref field, value, InitProtocol); }

    #endregion

    public string[] Protocols => field ??= [.. MyProtocols.Keys.OrderBy(x => x.Length())];
    public string[] Providers => field ??= [.. MyProviders.Keys.OrderBy(x => x.Length())];

    public string DefaultProtocol => field ??= MyProtocols.Values.Single(x => x is GodotNetwork).Name;
    public string DefaultProvider => field ??= MyProviders.Values.Single(x => x is LocalNetwork).Name;

    public event Action ProtocolChanged;
    public event Action ProviderChanged;

    public string Protocol { get; set => this.Set(ref field, GetValidProtocol(value), x => MyProtocol = MyProtocols[x], ProtocolChanged); }
    public string Provider { get; set => this.Set(ref field, GetValidProvider(value), x => MyProvider = MyProviders[x], ProviderChanged); }

    public string DefaultPlayerName => MyProvider.DefaultPlayerName;
    public int MaxSupportedPlayers => MyProvider.MaxSupportedPlayers;
    public string DefaultSessionName => App.Name;
    public const int DefaultPlayerLimit = 8;
    public const int MinPlayerLimit = 2;

    public event Action PlayerNameChanged;
    public event Action SessionNameChanged;
    public event Action MaxPlayersChanged;

    public string PlayerName { get; set => this.Set(ref field, GetValidPlayerName(value), PlayerNameChanged); }
    public string SessionName { get; set => this.Set(ref field, GetValidSessionName(value), SessionNameChanged); }
    public int MaxPlayers { get; set => this.Set(ref field, GetValidPlayerCount(value), MaxPlayersChanged); } = DefaultPlayerLimit;

    #region Private

    #region Utils

    private string GetValidProtocol(string candidate) => candidate.In(Protocols) ? candidate : DefaultProtocol;
    private string GetValidProvider(string candidate) => candidate.In(Providers) ? candidate : DefaultProvider;
    private string GetValidPlayerName(string candidate) => candidate.NotNullOrEmpty() ? candidate : DefaultPlayerName;
    private string GetValidSessionName(string candidate) => candidate.NotNullOrEmpty() ? candidate : DefaultSessionName;
    private int GetValidPlayerCount(int candidate) => candidate.Clamp(0, MaxSupportedPlayers);

    #endregion

    #region Init

    private IDictionary<string, T> InitLookup<T>(string root) where T : INetworkCommon
        => GetNode(root).GetChildren<T>(x => x.Exists).ToDictionary(x => (string)x.Name);

    private void InitProtocol(INetworkProtocol old, INetworkProtocol @new)
    {
        old?.Controller = null;
        @new?.Controller = MyProvider;
    }

    private void InitProtocol(INetworkProvider @new)
        => MyProtocol?.Controller = @new;

    #endregion

    #region Main

    private void InitServices()
    {
        InitMsgs();
        InitXtras();

        void InitMsgs()
        {
            IStatus.Subscribe(Name, DeliverMsg);
            MyProtocols.Keys.ForEach(key => IStatus.Subscribe(key, DeliverMsg));
            MyProviders.Keys.ForEach(key => IStatus.Subscribe(key, DeliverMsg));

            void DeliverMsg(Status status, string msg)
            {
                if (IsServer) ServerStatus?.Invoke(status, msg);
                if (IsClient) ClientStatus?.Invoke(status, msg);
                CombinedStatus?.Invoke(status, msg);

                if (status is Status.Error)
                {
                    if (IsServer) State = NetworkState.ServerError;
                    if (IsClient) State = NetworkState.ClientError;
                }
            }
        }

        void InitXtras()
        {
            Protocol = DefaultProtocol;
            Provider = DefaultProvider;

            var oldDefaultPlayerName = DefaultPlayerName;
            var oldMaxSupportedPlayers = MaxSupportedPlayers;

            ProviderChanged += () => { if (PlayerName == oldDefaultPlayerName) oldDefaultPlayerName = PlayerName = DefaultPlayerName; };
            ProviderChanged += () => { if (MaxPlayers == oldMaxSupportedPlayers) oldMaxSupportedPlayers = MaxPlayers = MaxSupportedPlayers; };
        }
    }

    #endregion

    #endregion
}
