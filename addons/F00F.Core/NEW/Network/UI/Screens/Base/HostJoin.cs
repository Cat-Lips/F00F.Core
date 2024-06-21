using Godot;

namespace F00F.NEW;

[Tool]
public /*abstract*/ partial class HostJoin : PopupView
{
    #region Private

    private PlayerName _PlayerName => field ??= (PlayerName)GetNode("%PlayerName");
    private ProviderSelect _Provider => field ??= (ProviderSelect)GetNode("%Provider");

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
                Network.Instance.StateChanged += ValidateChange;
            }

            void OnTreeExiting()
                => Network.Instance.StateChanged -= ValidateChange;
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
        _PlayerName.Text = App.AddTag(cfg.Get(PlayerName));
        _Provider.Selected = cfg.Get(Provider);
        _LoadState();
    }

    protected virtual void _SaveState() { }
    protected sealed override void SaveState()
    {
        var PlayerName = App.RemoveTag(this.PlayerName);
        cfg.Set(PlayerName);
        cfg.Set(Provider);
        _SaveState();
    }

    protected virtual bool _CanProceed() => true;
    protected sealed override bool CanProceed()
        => _CanProceed()
        && Network.Instance.NotActive
        && _PlayerName.Text.NotNullOrEmpty()
        && _Provider.Selected.NotNullOrEmpty();

    #endregion
}
