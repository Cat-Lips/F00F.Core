using System.Collections.Generic;
using Godot;

namespace F00F.NEW;

[Tool]
public partial class PlayerList : DataViewEx
{
    #region Private

    protected static Network Network => Network.Instance;

    private Label NetworkState => field ??= (Label)GetNode("%NetworkState");
    private StatusBar NetworkStatus => field ??= (StatusBar)GetNode("%NetworkStatus");
    private BaseButton ShowAllButton => field ??= (BaseButton)GetNode("%ToggleButton");
    private Separator StatusSep => field ??= (Separator)GetNode("%StatusSep");

    private record PlayerRow(TextureRect PlayerAvatar, Label PlayerName, Label PlayerLocation);
    private readonly Dictionary<ulong, PlayerRow> PlayerRows = [];
    private const int GridColumns = 3;

    #endregion

    #region Godot

    protected sealed override void OnReady()
    {
        if (Editor.IsEditor) return;
        Init(out var TitleDflt);
        InitControls();
        InitNetwork();

        void Init(out string TitleDflt)
        {
            TitleDflt = TitleText;
            Grid.Columns = GridColumns;
            GridMonitor = true;
        }

        void InitControls()
        {
            var HasPlayers = Network.HasPlayers;
            var ShowAllSet = ShowAllButton.ButtonPressed;
            ShowAllButton.Toggled += ShowAllToggled;
            GridActiveChanged += () => HasPlayersChanged(GridActive);
            MouseOverChanged += UpdateView;
            UpdateView();

            void HasPlayersChanged(bool hasPlayers)
            {
                HasPlayers = hasPlayers;
                UpdateView();
            }

            void ShowAllToggled(bool showAll)
            {
                ShowAllSet = showAll;
                UpdateView();
            }

            void UpdateView()
            {
                ShowXtras = HasPlayers && ShowAllSet;
                StatusSep.Visible = MouseOver || ShowAllSet;
                NetworkStatus.Visible = MouseOver || ShowAllSet;
                ResetSize();
            }
        }

        void InitNetwork()
        {
            OnTreeEntered();
            TreeEntered += OnTreeEntered;
            TreeExiting += OnTreeExiting;

            #region Tree

            void OnTreeEntered()
            {
                InitPlayers();
                InitSession();

                Network.PlayerInsert += InsertRow;
                Network.PlayerUpdate += UpdateRow;
                Network.PlayerRemove += RemoveRow;
                Network.PlayersReset += ClearRows;

                Network.StateChanged += SetState;
                Network.SessionUpdate += SetTitle;
                Network.CombinedStatus += SetStatus;
            }

            void OnTreeExiting()
            {
                Network.PlayerInsert -= InsertRow;
                Network.PlayerUpdate -= UpdateRow;
                Network.PlayerRemove -= RemoveRow;
                Network.PlayersReset -= ClearRows;

                Network.StateChanged -= SetState;
                Network.SessionUpdate -= SetTitle;
                Network.CombinedStatus += SetStatus;
            }

            #endregion

            #region Init

            void InitPlayers()
            {
                Network.CurrentPlayers?.ForEach(kvp =>
                {
                    InsertRow(kvp.Key);
                    UpdateRow(kvp.Value);
                });
            }

            void InitSession()
            {
                SetState();
                SetTitle();
            }

            #endregion

            #region Players

            void InsertRow(ulong id)
            {
                var row = new PlayerRow(PlayerAvatar(), PlayerName(), PlayerLocation());
                PlayerRows.Add(id, row);

                Grid.AddChild(row.PlayerAvatar);
                Grid.AddChild(row.PlayerName);
                Grid.AddChild(row.PlayerLocation);

                Label PlayerName() => new() { Name = "PlayerName", };
                Label PlayerLocation() => new() { Name = "PlayerLocation", }; // todo: flag icon?
                TextureRect PlayerAvatar() => new() { Name = "PlayerAvatar", };
            }

            void UpdateRow(Network.PlayerProfile pp)
            {
                if (PlayerRows.TryGetValue(pp.PlayerId, out var row))
                {
                    row.PlayerName.Text = pp.PlayerName;
                    row.PlayerAvatar.Texture = pp.PlayerAvatar;
                    row.PlayerLocation.Text = $"({pp.PlayerLocation})";
                }
            }

            void RemoveRow(ulong id, string reason)
            {
                if (PlayerRows.Remove(id, out var row))
                {
                    Grid.RemoveChild(row.PlayerName, free: true);
                    Grid.RemoveChild(row.PlayerAvatar, free: true);
                    Grid.RemoveChild(row.PlayerLocation, free: true);
                    NetworkStatus.SetStatus(Status.Info, $"{row.PlayerName.Text} {reason}");
                }
            }

            void ClearRows()
            {
                PlayerRows.Clear();
                Grid.RemoveChildren();
            }

            #endregion

            #region Session

            void SetState()
            {
                Visible = Network.HasState;
                NetworkState.Text = Network.SessionType();
                NetworkState.SetFontColor(Network.SessionStatus().FontColor());
                NetworkState.SetBackColor(Network.SessionStatus().BackColor());
            }

            void SetTitle()
            {
                var title = Network.SessionTitle(TitleDflt);
                ShowAllButton.TooltipText = title;
                TitleText = title;
            }

            void SetStatus(Status status, string msg)
                => NetworkStatus.SetStatus(status, msg);

            #endregion
        }
    }

    #endregion
}
