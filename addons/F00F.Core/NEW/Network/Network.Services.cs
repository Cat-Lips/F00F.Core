using System;
using System.Collections.Generic;
using System.Linq;

namespace F00F.NEW;

public partial class Network
{
    #region Private

    private IDictionary<string, INetworkProtocol> ProtocolLookup;
    private IDictionary<string, INetworkProvider> ProviderLookup;

    private INetworkProtocol CoreProtocol;
    private INetworkProvider CoreProvider;

    private Config<Network> cfg = new();

    #endregion

    public event Action ActiveProtocolsChanged;
    public event Action ActiveProvidersChanged;
    public event Action CurrentProtocolChanged;
    public event Action CurrentProviderChanged;

    public string[] AvailableProtocols { get; private set; }
    public string[] AvailableProviders { get; private set; }
    public string[] ActiveProtocols { get; private set => this.Set(ref field, value, ActiveProtocolsChanged); }
    public string[] ActiveProviders { get; private set => this.Set(ref field, value, ActiveProvidersChanged); }

    public string DefaultProtocol { get; private set; }
    public string DefaultProvider { get; private set; }
    public string CurrentProtocol { get; set => this.Set(ref field, GetValidProtocol(value), CurrentProtocolChanged); }
    public string CurrentProvider { get; set => this.Set(ref field, GetValidProvider(value), CurrentProviderChanged); }

    public string DefaultPlayerName => CoreProvider.DefaultPlayerName;
    public int MaxSupportedPlayers => CoreProvider.MaxSupportedPlayers;
    public string DefaultGameName => $"[{App.Name}] {DefaultPlayerName}";
    public const int DefaultPlayerLimit = 8;

    public event Action LocalPlayerNameChanged;
    public event Action MaxAllowedPlayersChanged;
    public event Action CurrentGameNameChanged;

    public string LocalPlayerName { get; set => this.Set(ref field, value.IsNullOrEmpty() ? DefaultPlayerName : value, LocalPlayerNameChanged); }
    public int MaxAllowedPlayers { get; set => this.Set(ref field, value.Clamp(1, MaxSupportedPlayers), MaxAllowedPlayersChanged); } = DefaultPlayerLimit;
    public string CurrentGameName { get; set => this.Set(ref field, value.IsNullOrEmpty() ? DefaultGameName : value, CurrentGameNameChanged); }

    #region Private

    private string GetValidProtocol(string candidate) => ProtocolLookup.ContainsKey(candidate ?? "") ? candidate : DefaultProtocol;
    private string GetValidProvider(string candidate) => ProviderLookup.ContainsKey(candidate ?? "") ? candidate : DefaultProvider;

    private void InitServices()
    {
        InitLookups();
        InitDefaults();
        InitAvailable();
        InitActive();
        InitCurrent();
        InitCore();
        InitCfg();
        Xtras();
        Msgs();

        void InitLookups()
        {
            ProtocolLookup = GetNode("Protocols").GetChildren<INetworkProtocol>(x => x.Exists).ToDictionary(x => (string)x.Name);
            ProviderLookup = GetNode("Providers").GetChildren<INetworkProvider>(x => x.Exists).ToDictionary(x => (string)x.Name);
        }

        void InitDefaults()
        {
            DefaultProtocol = ProtocolLookup.Values.Single(x => x is GodotNetwork).Name;
            DefaultProvider = ProviderLookup.Values.Single(x => x is LocalNetwork).Name;
        }

        void InitAvailable()
        {
            AvailableProtocols = [.. ProtocolLookup.Keys.OrderBy(x => x.Length())];
            AvailableProviders = [.. ProviderLookup.Keys.OrderBy(x => x.Length())];
        }

        void InitActive()
        {
            var SetActiveProtocolsAction = new AutoAction(() => ActiveProtocols = [.. ProtocolLookup.Values.Where(x => x.Active).Select(x => x.Name).OrderBy(x => x.Length())]);
            var SetActiveProvidersAction = new AutoAction(() => ActiveProviders = [.. ProviderLookup.Values.Where(x => x.Active).Select(x => x.Name).OrderBy(x => x.Length())]);

            ProtocolLookup.Values.ForEach(x => x.ActiveChanged += SetActiveProtocolsAction.Run);
            ProviderLookup.Values.ForEach(x => x.ActiveChanged += SetActiveProvidersAction.Run);

            SetActiveProtocolsAction.RunNow();
            SetActiveProvidersAction.RunNow();
        }

        void InitCurrent()
        {
            CurrentProtocol = GetValidProtocol(CurrentProtocol);
            CurrentProvider = GetValidProvider(CurrentProvider);

            ActiveProtocolsChanged += () => CurrentProtocol = GetValidProtocol(CurrentProtocol);
            ActiveProvidersChanged += () => CurrentProvider = GetValidProvider(CurrentProvider);
        }

        void InitCore()
        {
            CoreProtocol = ProtocolLookup[CurrentProtocol];
            CoreProvider = ProviderLookup[CurrentProvider];

            CurrentProtocolChanged += () => CoreProtocol = ProtocolLookup[CurrentProtocol];
            CurrentProviderChanged += () => CoreProvider = ProviderLookup[CurrentProvider];
        }

        void InitCfg()
        {
            CurrentProtocol = cfg.Get(CurrentProtocol);
            CurrentProvider = cfg.Get(CurrentProvider);

            LocalPlayerName = cfg.Get(LocalPlayerName);
            MaxAllowedPlayers = cfg.Get(MaxAllowedPlayers);
            CurrentGameName = cfg.Get(CurrentGameName);

            CurrentProtocolChanged += () => cfg.Set(CurrentProtocol);
            CurrentProviderChanged += () => cfg.Set(CurrentProvider);

            LocalPlayerNameChanged += () => cfg.Set(LocalPlayerName);
            MaxAllowedPlayersChanged += () => cfg.Set(MaxAllowedPlayers);
            CurrentGameNameChanged += () => cfg.Set(CurrentGameName);
        }

        void Xtras()
        {
            var oldDefaultPlayerName = DefaultPlayerName;
            var oldMaxSupportedPlayers = MaxSupportedPlayers;
            var oldDefaultGameName = DefaultGameName;

            CurrentProviderChanged += () => { if (LocalPlayerName == oldDefaultPlayerName) oldDefaultPlayerName = LocalPlayerName = DefaultPlayerName; };
            CurrentProviderChanged += () => { if (MaxAllowedPlayers == oldMaxSupportedPlayers) oldMaxSupportedPlayers = MaxAllowedPlayers = MaxSupportedPlayers; };
            CurrentProviderChanged += () => { if (CurrentGameName == oldDefaultGameName) oldDefaultGameName = CurrentGameName = DefaultGameName; };
            LocalPlayerNameChanged += () => { if (CurrentGameName == oldDefaultGameName) oldDefaultGameName = CurrentGameName = DefaultGameName; };
        }

        void Msgs()
        {
            IStatus.Subscribe(Name, DeliverMsg);
            ProtocolLookup.Keys.ForEach(key => IStatus.Subscribe(key, DeliverMsg));
            ProviderLookup.Keys.ForEach(key => IStatus.Subscribe(key, DeliverMsg));

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
    }

    #endregion
}
