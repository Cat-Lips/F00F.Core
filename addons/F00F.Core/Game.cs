using System;
using Godot;

namespace F00F;

public partial class Game : Node
{
    protected GUI UI => field ??= GetNode<GUI>("UI");
    protected Network Network => field ??= GetNode<Network>("Network");

    protected IPlayerData PlayerData => UI.PlayerData;

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
            => UI.MainMenu.EnablePlayerName(PlayerData);

        void EnablePlayerColor()
            => UI.MainMenu.EnablePlayerColor(PlayerData);

        void EnablePlayerAvatars()
        {
            UI.MainMenu.EnablePlayerAvatars(PlayerData, cfg.AvatarIcons);
            UI.MainMenu.EnablePlayerAvatars(PlayerData, cfg.PlayerAvatars);
        }
    }

    protected virtual void InitGame<TPlayer>(Action<TPlayer> InitLocalPlayer) where TPlayer : Node => InitGame(null, null, InitLocalPlayer);
    protected virtual void InitGame<TPlayer>(GameConfig cfg, Action<TPlayer> InitLocalPlayer = null) where TPlayer : Node => InitGame(cfg, null, InitLocalPlayer);
    protected virtual void InitGame<TPlayer>(GameConfig cfg, Node world, Action<TPlayer> InitLocalPlayer = null) where TPlayer : Node
    {
        cfg ??= new();
        InitGame(cfg);
        InitNetwork();

        void InitNetwork()
        {
            UI.MainMenu.EnableNetworkMenus(Network);
            UI.PlayerList.Initialise(cfg.PlayerData);
            Network.Initialise<TPlayer>(world, OnPlayerAdded, OnPlayerRemoved);

            void OnPlayerAdded(int pid, TPlayer player)
            {
                UI.AddPlayer(pid);

                if (player.IsLocal())
                    InitLocalPlayer?.Invoke(player);
            }

            void OnPlayerRemoved(int pid, TPlayer _)
                => UI.RemovePlayer(pid);
        }
    }

    protected virtual void OnReady() { }
    public sealed override void _Ready()
    {
        Editor.Disable(this);
        if (Editor.IsEditor) return;

        ParseCmdLine();
        OnReady();

        void ParseCmdLine()
            => CmdLine.Parse(Network);
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
