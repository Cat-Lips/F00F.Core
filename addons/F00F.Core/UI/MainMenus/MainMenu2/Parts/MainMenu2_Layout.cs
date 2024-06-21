using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace F00F;

[Tool]
public partial class MainMenu2_Layout : Container, IMainMenu
{
    private readonly Config cfg = new("MainMenu");

    #region Scene

    private Button Go => field ??= GetNode<Button>("%Go");
    private Button Quit => field ??= GetNode<Button>("%Quit");
    private Button Help => field ??= GetNode<Button>("%Help");

    private Button ServerToggle => field ??= GetNode<Button>("%ServerToggle");
    private Button ClientToggle => field ??= GetNode<Button>("%ClientToggle");
    private Button PlayerSetupToggle => field ??= GetNode("%PlayerSetupToggle").GetNode<Button>("Toggle");
    private Button GameOptionsToggle => field ??= GetNode("%GameOptionsToggle").GetNode<Button>("Toggle");

    private Container ServerMenu => field ??= GetNode<Container>("%ServerMenu");
    private Container ClientMenu => field ??= GetNode<Container>("%ClientMenu");
    private Container PlayerSetupMenu => field ??= GetNode<Container>("%PlayerSetupMenu");
    private GridContainer GameOptionsMenu => field ??= GetNode<GridContainer>("%GameOptionsMenu");

    private LineEdit ServerAddress => field ??= GetNode<LineEdit>("%ServerAddress");
    private PortEdit ServerPort => field ??= GetNode<PortEdit>("%ServerPort");
    private LineEdit ConnectAddress => field ??= GetNode<LineEdit>("%ConnectAddress");
    private PortEdit ConnectPort => field ??= GetNode<PortEdit>("%ConnectPort");

    private Label PlayerNameLabel => field ??= GetNode<Label>("%PlayerNameLabel");
    private Label PlayerColorLabel => field ??= GetNode<Label>("%PlayerColorLabel");
    private Label PlayerAvatarLabel => field ??= GetNode<Label>("%PlayerAvatarLabel");

    private LineEdit PlayerName => field ??= GetNode<LineEdit>("%PlayerName");
    private ColorPickerButton PlayerColor => field ??= GetNode<ColorPickerButton>("%PlayerColor");
    private OptionButton PlayerAvatar => field ??= GetNode<OptionButton>("%PlayerAvatar");

    private Separator NetworkSep => field ??= GetNode<Separator>("%NetworkSep");
    private Separator ConfigSep => field ??= GetNode<Separator>("%ConfigSep");
    private Separator ButtonsSep => field ??= GetNode<Separator>("%ButtonsSep");
    private Separator HelpQuitSep => field ??= GetNode<Separator>("%ButtonSep");

    #endregion

    public event Action Start;

    public void EnableNetworkMenus(Network network)
    {
        NetworkSep.Visible = true;
        ServerToggle.Visible = true;
        ClientToggle.Visible = true;

        // NB: ButtonGroup toggles off when other is on
        ServerToggle.Toggled += InitServer;
        ClientToggle.Toggled += InitClient;

        void InitServer(bool on)
        {
            ServerMenu.Visible = on;
            ClientToggle.Visible = !on;
            if (on) Start += HostGame;
            else Start -= HostGame;

            void HostGame()
                => network.StartServer(ServerPort.Value);
        }

        void InitClient(bool on)
        {
            ClientMenu.Visible = on;
            ServerToggle.Visible = !on;
            if (on) Start += JoinGame;
            else Start -= JoinGame;

            void JoinGame()
                => network.CreateClient(ConnectAddress.Text, ConnectPort.Value);
        }
    }

    public void EnablePlayerName(IPlayerData data)
    {
        LoadPlayerName();
        EnablePlayerName();

        void LoadPlayerName()
        {
            var name = _LoadPlayerName();
            PlayerName.Text = name;
            data.PlayerName = name;
        }

        void EnablePlayerName()
        {
            ConfigSep.Visible = true;
            PlayerName.Visible = true;
            PlayerNameLabel.Visible = true;
            PlayerSetupToggle.Visible = true;

            PlayerName.LoseFocusOnEnter();
            PlayerName.TextChanged += OnPlayerNameEdited;
            PlayerName.FocusExited += OnPlayerNameChanged;

            void OnPlayerNameEdited(string edit)
                => Go.Enabled(IsValidPlayerName(edit));

            void OnPlayerNameChanged()
            {
                var name = PlayerName.Text;
                if (!IsValidPlayerName(name))
                    PlayerName.Text = name = _LoadPlayerName();
                _SavePlayerName(name);
                data.PlayerName = name;
            }

            bool IsValidPlayerName(string name)
                => !string.IsNullOrWhiteSpace(name) && name != PlayerName.PlaceholderText;
        }

        void _SavePlayerName(string name)
            => cfg.Set(this, PlayerName, App.RemoveId(name));

        string _LoadPlayerName()
            => App.AddId(cfg.Get(this, PlayerName, _DefaultPlayerName()));

        static string _DefaultPlayerName()
            => System.Environment.UserName;
    }

    public void EnablePlayerColor(IPlayerData data)
    {
        LoadPlayerColor();
        EnablePlayerColor();

        void LoadPlayerColor()
        {
            var color = cfg.Get(this, PlayerColor, Const.DefaultColors.PickRandom());
            PlayerColor.Color = data.PlayerColor = color;
        }

        void EnablePlayerColor()
        {
            ConfigSep.Visible = true;
            PlayerColor.Visible = true;
            PlayerColorLabel.Visible = true;
            PlayerSetupToggle.Visible = true;

            PlayerColor.ColorChanged += OnColorChanged;

            void OnColorChanged(Color color)
            {
                cfg.Set(this, PlayerColor, color);
                data.PlayerColor = color;
            }
        }
    }

    public void EnablePlayerAvatars(IPlayerData data, params string[] avatars)
    {
        if (!avatars.IsNullOrEmpty())
        {
            PlayerAvatar.AddItems(avatars);
            SetPlayerAvatar(data, avatars.PickRandom());
        }
    }

    public void EnablePlayerAvatars(IPlayerData data, params (string Key, Texture2D Icon)[] avatars)
    {
        if (!avatars.IsNullOrEmpty())
        {
            PlayerAvatar.AddItems(avatars);
            SetPlayerAvatar(data, avatars.PickRandom().Key);
        }
    }

    private void SetPlayerAvatar(IPlayerData data, string @default)
    {
        LoadPlayerAvatar();
        EnablePlayerAvatar();

        void LoadPlayerAvatar()
        {
            var avatar = cfg.Get(this, PlayerAvatar, @default);
            PlayerAvatar.Selected = PlayerAvatar.IndexOf(avatar);
            data.PlayerAvatar = avatar;
        }

        void EnablePlayerAvatar()
        {
            ConfigSep.Visible = true;
            PlayerAvatar.Visible = true;
            PlayerAvatarLabel.Visible = true;
            PlayerSetupToggle.Visible = true;

            PlayerAvatar.ItemSelected += OnAvatarChanged;

            void OnAvatarChanged(long idx)
            {
                var avatar = (string)PlayerAvatar.GetItemMetadata((int)idx);
                cfg.Set(this, PlayerAvatar, avatar);
                data.PlayerAvatar = avatar;
            }
        }
    }

    public void EnableGameOptions(params IEnumerable<(string Label, Control Control)> items)
    {
        ConfigSep.Visible = true;
        GameOptionsToggle.Visible = true;

        GameOptionsMenu.Init(items.Select(x => (UI.NewLabel(x.Label) as Control, x.Control)));
    }

    public void EnableGameOptions(string group, params IEnumerable<(string Label, Control Control)> items)
    {
        // TODO
        //ConfigSep.Visible = true;
        //GameOptionsToggle.Visible = true;

        //GameOptionsMenu.Init(items.Select(x => (UI.NewLabel(x.Label) as Control, x.Control)));
    }

    public void EnableQuickHelp(RootPopup help)
    {
        Help.Visible = true;
        Help.Pressed += help.Show;
        ButtonsSep.Visible = true;
        HelpQuitSep.Visible = Quit.Visible;
    }

    public void EnableQuitGame()
    {
        Quit.Visible = true;
        Quit.Pressed += OnQuit;
        ButtonsSep.Visible = true;
        HelpQuitSep.Visible = Help.Visible;

        void OnQuit()
            => this.Quit();
    }

    #region Godot

    public sealed override void _Ready()
    {
        InitStart();
        this.InitQuit();
        SetVisibility(Editor.IsEditor);

        void InitStart()
        {
            Go.Pressed += OnStart;

            void OnStart()
            {
                Hide();
                Start?.Invoke();
            }
        }

        void SetVisibility(bool visible)
        {
            Quit.Visible = visible;
            Help.Visible = visible;

            ServerToggle.Visible = visible;
            ClientToggle.Visible = visible;
            PlayerSetupToggle.Visible = visible;
            GameOptionsToggle.Visible = visible;

            ServerMenu.Visible = visible;
            ClientMenu.Visible = visible;
            PlayerSetupMenu.Visible = visible;
            GameOptionsMenu.Visible = visible;

            PlayerName.Visible = visible;
            PlayerColor.Visible = visible;
            PlayerAvatar.Visible = visible;
            PlayerNameLabel.Visible = visible;
            PlayerColorLabel.Visible = visible;
            PlayerAvatarLabel.Visible = visible;

            ConfigSep.Visible = visible;
            NetworkSep.Visible = visible;
            ButtonsSep.Visible = visible;
            HelpQuitSep.Visible = visible;
        }
    }

    #endregion
}
