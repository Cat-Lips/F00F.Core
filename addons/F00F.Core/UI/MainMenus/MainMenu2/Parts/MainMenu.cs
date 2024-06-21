using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace F00F;

[Tool]
public partial class MainMenu : Container, IMainMenu
{
    #region Private

    private readonly Config cfg = new("MainMenu");

    private Button Go => field ??= (Button)GetNode("%Go");
    private Button Quit => field ??= (Button)GetNode("%Quit");
    private Button Help => field ??= (Button)GetNode("%Help");

    private Button ServerToggle => field ??= (Button)GetNode("%ServerToggle");
    private Button ClientToggle => field ??= (Button)GetNode("%ClientToggle");
    private Button PlayerSetupToggle => field ??= (Button)GetNode("%PlayerSetupToggle");
    private Button GameOptionsToggle => field ??= (Button)GetNode("%GameOptionsToggle");

    private Container ServerMenu => field ??= (Container)GetNode("%ServerMenu");
    private Container ClientMenu => field ??= (Container)GetNode("%ClientMenu");
    private GridContainer PlayerSetupMenu => field ??= (GridContainer)GetNode("%PlayerSetupMenu");
    private GridContainer GameOptionsMenu => field ??= (GridContainer)GetNode("%GameOptionsMenu");

    private LineEdit ServerAddress => field ??= (LineEdit)GetNode("%ServerAddress");
    private PortEdit ServerPort => field ??= (PortEdit)GetNode("%ServerPort");
    private LineEdit ConnectAddress => field ??= (LineEdit)GetNode("%ConnectAddress");
    private PortEdit ConnectPort => field ??= (PortEdit)GetNode("%ConnectPort");

    private Label PlayerNameLabel => field ??= (Label)GetNode("%PlayerNameLabel");
    private Label PlayerColorLabel => field ??= (Label)GetNode("%PlayerColorLabel");
    private Label PlayerAvatarLabel => field ??= (Label)GetNode("%PlayerAvatarLabel");

    private LineEdit PlayerName => field ??= (LineEdit)GetNode("%PlayerName");
    private ColorPickerButton PlayerColor => field ??= (ColorPickerButton)GetNode("%PlayerColor");
    private OptionButton PlayerAvatar => field ??= (OptionButton)GetNode("%PlayerAvatar");

    private Separator NetworkSep => field ??= (Separator)GetNode("%NetworkSep");
    private Separator ConfigSep => field ??= (Separator)GetNode("%ConfigSep");
    private Separator ButtonsSep => field ??= (Separator)GetNode("%ButtonsSep");
    private Separator HelpQuitSep => field ??= (Separator)GetNode("%Buttons").GetNode("Sep");

    #endregion

    public event Action Start;

    #region Config

    public void EnableNetworkMenus(Network network)
    {
        SetStartText();
        NetworkSep.Visible = true;
        ServerToggle.Visible = true;
        ClientToggle.Visible = true;

        // NB: ButtonGroup toggles off when other is on
        ServerToggle.Toggled += InitServer;
        ClientToggle.Toggled += InitClient;

        void InitServer(bool on)
        {
            SetStartText();
            ServerMenu.Visible = on;
            ClientToggle.Visible = !on;
            if (on) Start += HostGame;
            else Start -= HostGame;

            void HostGame()
                => network.StartServer(ServerPort.Value);
        }

        void InitClient(bool on)
        {
            SetStartText();
            ClientMenu.Visible = on;
            ServerToggle.Visible = !on;
            if (on) Start += JoinGame;
            else Start -= JoinGame;

            void JoinGame()
                => network.CreateClient(ConnectAddress.Text, ConnectPort.Value);
        }

        void SetStartText()
        {
            Go.Text =
                ServerToggle.ButtonPressed ? "Host Multiplayer Game" :
                ClientToggle.ButtonPressed ? "Join Multiplayer Game" :
                "Start Singleplayer Game";

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
        if (!items.IsNullOrEmpty())
        {
            ConfigSep.Visible = true;
            GameOptionsToggle.Visible = true;

            GameOptionsMenu.Init(items.Select(x => (UI.NewLabel(x.Label) as Control, x.Control)));
        }
    }

    public void EnableGameOptions(string group, params IEnumerable<(string Label, Control Control)> items)
    {
        if (!items.IsNullOrEmpty())
        {
            // TODO
            //ConfigSep.Visible = true;
            //GameOptionsToggle.Visible = true;

            //GameOptionsMenu.Init(items.Select(x => (UI.NewLabel(x.Label) as Control, x.Control)));
        }
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
            => App.Quit(this);
    }

    #endregion

    #region Godot

    public sealed override void _Ready()
    {
        InitButtons();
        SetVisibility();

        void InitButtons()
        {
            Go.Pressed += () => Start?.Invoke();

            // NB: ButtonGroup toggles off when other is on
            PlayerSetupToggle.Toggled += on => PlayerSetupMenu.Visible = on;
            GameOptionsToggle.Toggled += on => GameOptionsMenu.Visible = on;
        }

        void SetVisibility()
        {
            if (Editor.IsEditor) return;

            this.ExpandToFitWidth();

            Quit.Visible = false;
            Help.Visible = false;

            ServerToggle.Visible = false;
            ClientToggle.Visible = false;
            PlayerSetupToggle.Visible = false;
            GameOptionsToggle.Visible = false;

            ServerMenu.Visible = false;
            ClientMenu.Visible = false;
            PlayerSetupMenu.Visible = false;
            GameOptionsMenu.Visible = false;

            PlayerName.Visible = false;
            PlayerColor.Visible = false;
            PlayerAvatar.Visible = false;
            PlayerNameLabel.Visible = false;
            PlayerColorLabel.Visible = false;
            PlayerAvatarLabel.Visible = false;

            ConfigSep.Visible = false;
            NetworkSep.Visible = false;
            ButtonsSep.Visible = false;
            HelpQuitSep.Visible = false;
        }
    }

    #endregion
}
