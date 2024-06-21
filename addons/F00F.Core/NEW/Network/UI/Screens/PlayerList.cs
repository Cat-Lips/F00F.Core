using System;
using System.Diagnostics;
using Godot;

namespace F00F.NEW;

using static Network;

[Tool]
public partial class PlayerList : DataViewEx
{
    #region Private

    protected static Network Network => Network.Instance;

    private Button ShowButton => field ??= (Button)GetNode("%ShowAllButton");
    private Control[] ShowTargets => field ??= [
        (Control)GetNode("%Title"),
        (Control)GetNode("%Sep1"),
        (Control)GetNode("%Scroll"),
        (Control)GetNode("%Sep2")];

    private Label NetworkState => field ??= (Label)GetNode("%NetworkState");
    private StatusBar NetworkStatus => field ??= (StatusBar)GetNode("%NetworkStatus");

    #endregion

    public event Action PlayersChanged;
    public PlayerProfile[] Players { get; private set => this.Set(ref field, value, PlayersChanged); }

    #region Godot

    protected sealed override void OnReady()
    {
        if (Editor.NotEditor)
        {
            InitButtons();
            InitNetwork();
        }

        void InitButtons()
        {
            ShowPlayers();
            PlayersChanged += ShowPlayers;
            ShowButton.Toggled += _ => ShowPlayers();

            void ShowPlayers()
            {
                var HasPlayers = Players.NotNullOrEmpty();
                var ShowPressed = ShowButton.ButtonPressed;

                ShowButton.Visible = HasPlayers;

                var show = HasPlayers && ShowPressed;
                ShowTargets.ForEach(x => x.Visible = show);
                if (!show) ResetSize();
            }
        }

        void InitNetwork()
        {
            OnTreeEntered();
            TreeEntered += OnTreeEntered;
            TreeExiting += OnTreeExiting;

            void OnTreeEntered()
            {
                OnStateChanged();
                Network.StateChanged += OnStateChanged;
                Network.ClientStatus += SetClientStatus;
                Network.ServerStatus += SetServerStatus;
            }

            void OnTreeExiting()
            {
                Network.StateChanged -= OnStateChanged;
                Network.ClientStatus -= SetClientStatus;
                Network.ServerStatus -= SetServerStatus;
            }

            void OnStateChanged()
            {
                NetworkState.Text =
                    Network.IsClient ? "CLIENT" :
                    Network.IsServer ? "SERVER" : null;
                NetworkState.SetFontColor(
                    Network.Success ? Status.Success.Color() :
                    Network.Error ? Status.Error.Color() :
                    Network.Warn ? Status.Error.Color() : null);
                if (Network.IsReset) NetworkStatus.ClearStatus();
                Visible = Network.HasState;
            }

            void SetClientStatus(Status status, string msg)
            {
                Debug.Assert(NetworkState.Text is "CLIENT");
                NetworkStatus.SetStatus(status, msg);
            }

            void SetServerStatus(Status status, string msg)
            {
                Debug.Assert(NetworkState.Text is "SERVER");
                NetworkStatus.SetStatus(status, msg);
            }
        }
    }

    #endregion
}
