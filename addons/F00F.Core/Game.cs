using System;
using Godot;

namespace F00F;

public partial class Game : Node
{
    protected GUI UI => field ??= GetNode<GUI>("UI");
    protected Network Network => field ??= GetNode<Network>("Network");

    protected virtual void InitGame(GameConfig cfg = null)
    {
        cfg ??= new();

        if (cfg.EnableQuitGame) EnableQuitGame();
        if (cfg.EnableQuickHelp) EnableQuickHelp();
        if (cfg.EnableGameOptions) EnableGameOptions();

        if (cfg.EnablePlayerName) EnablePlayerName();
        if (cfg.EnablePlayerColor) EnablePlayerColor();
        if (cfg.EnablePlayerAvatars) EnablePlayerAvatars();

        void EnableQuitGame()
            => UI.MainMenu.EnableQuitGame();

        void EnableQuickHelp()
        {
            UI.QuickHelp.TextFile = cfg.QuickHelp;
            UI.MainMenu.EnableGameOptions(("Quick Help...", UI.QuickHelp.Show));
        }

        void EnableGameOptions()
        {
            UI.MainMenu.EnableGameOptions(cfg.GameOptions);
            UI.MainMenu.EnableGameOptions(cfg.AddGameOptions);
        }

        void EnablePlayerName()
            => UI.MainMenu.EnablePlayerName();

        void EnablePlayerColor()
            => UI.MainMenu.EnablePlayerColor();

        void EnablePlayerAvatars()
        {
            UI.MainMenu.EnablePlayerAvatars(cfg.AvatarIcons);
            UI.MainMenu.EnablePlayerAvatars(cfg.PlayerAvatars);
        }
    }

    protected virtual void InitGame<TPlayer>(Action<TPlayer> InitPlayer, Action<TPlayer, IPlayerData> InitLocalPlayer = null) where TPlayer : Node => InitGame(null, null, InitPlayer, InitLocalPlayer);
    protected virtual void InitGame<TPlayer>(GameConfig cfg, Action<TPlayer> InitPlayer = null, Action<TPlayer, IPlayerData> InitLocalPlayer = null) where TPlayer : Node => InitGame(cfg, null, InitPlayer, InitLocalPlayer);
    protected virtual void InitGame<TPlayer>(GameConfig cfg, Node world, Action<TPlayer> InitPlayer = null, Action<TPlayer, IPlayerData> InitLocalPlayer = null) where TPlayer : Node
    {
        InitGame(cfg ??= new());
        InitNetwork();
        ParseCmdLine();

        void InitNetwork()
        {
            UI.MainMenu.EnableNetworkMenus(Network);
            UI.PlayerList.Initialise(cfg.PlayerData);
            Network.Initialise<TPlayer>(world, OnPlayerAdded, OnPlayerRemoved);

            void OnPlayerAdded(TPlayer player)
            {
                InitPlayer?.Invoke(player);

                var data = UI.PlayerList.AddPlayer(player);
                var local = player.IsAuth();

                if (local)
                {
                    data.DisplayName = UI.MainMenu.PlayerName;
                    if (cfg.EnablePlayerColor)
                        data.DisplayColor = UI.MainMenu.PlayerColor;
                    InitLocalPlayer?.Invoke(player, data);
                }
                else
                {
                    data.DisplayName = player.Name;
                }
            }

            void OnPlayerRemoved(TPlayer player)
                => UI.PlayerList.RemovePlayer(player);
        }

        void ParseCmdLine()
            => CmdLine.Parse(Network);
    }

    protected virtual void OnReady() { }
    public sealed override void _Ready()
    {
        Editor.Disable(this);
        if (Editor.IsEditor) return;

        OnReady();
    }

    #region Config

    protected class GameConfig
    {
        public virtual bool EnableQuitGame => OS.HasFeature("pc");
        public virtual bool EnableQuickHelp => FS.FileExists(QuickHelp);
        public virtual bool EnableGameOptions => true;

        public virtual bool EnablePlayerName => true;
        public virtual bool EnablePlayerColor => true;
        public virtual bool EnablePlayerAvatars => true;

        public virtual string[] PlayerAvatars => null;
        public virtual (string Key, Texture2D Icon)[] AvatarIcons => null;

        public virtual (string Label, Action Action)[] GameOptions => null;
        public virtual void AddGameOptions(PopupMenu root) { }

        public virtual string[] PlayerData => null;
        public virtual string QuickHelp => "res://Assets/Help/QuickHelp.txt";
    }

    #endregion
}
