using Godot;

namespace F00F.NEW;

[Tool]
public /*abstract*/ partial class HostJoin : PopupView
{
    #region Private

    protected static Network Network => Network.Instance;

    private PlayerName _PlayerName => field ??= (PlayerName)GetNode("%PlayerName");
    private ActiveProviders _Provider => field ??= (ActiveProviders)GetNode("%Provider");

    #endregion

    public string PlayerName => _PlayerName.Text;
    public string Provider => _Provider.Selected;

    #region Godot

    protected sealed override void OnReady()
    {
        ReadyNetwork();
        ReadyControls();

        void ReadyNetwork()
        {
            OnTreeEntered();
            TreeEntered += OnTreeEntered;
            TreeExiting += OnTreeExiting;

            void OnTreeEntered()
            {
                ValidateChange();
                Network.StateChanged += ValidateChange;
            }

            void OnTreeExiting()
                => Network.StateChanged -= ValidateChange;
        }

        void ReadyControls()
        {
            _PlayerName.TextChanged += ValidateChange;
            _Provider.ItemSelected += ValidateChange;
        }
    }

    #endregion

    #region Private

    protected virtual void _LoadState() { }
    protected sealed override void LoadState()
    {
        _PlayerName.Text = cfg.Get(PlayerName);
        _Provider.Selected = cfg.Get(Provider);
        _LoadState();
    }

    protected virtual void _SaveState() { }
    protected sealed override void SaveState()
    {
        cfg.Set(PlayerName);
        cfg.Set(Provider);
        _SaveState();
    }

    protected virtual bool _CanProceed() => true;
    protected sealed override bool CanProceed()
        => _CanProceed()
        && Network.NotActive
        && _PlayerName.Text.NotNullOrEmpty()
        && _Provider.Selected.NotNullOrEmpty();

    #endregion
}
