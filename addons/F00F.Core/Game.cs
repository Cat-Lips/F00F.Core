using System;
using Godot;

namespace F00F;

public partial class Game : Node
{
    #region Private

    protected GUI UI => field ??= GetNode<GUI>("UI");
    protected Network Network => field ??= GetNode<Network>("Network");

    #endregion

    protected virtual IPlayerData PlayerData => UI.PlayerData;
    protected virtual IGameConfig GameConfig => field ??= new GameConfig();

    protected void InitGame<TPlayer>(Node world = null, Action<TPlayer> InitGame = null, Action<TPlayer, Color> SetPlayerColor = null) where TPlayer : Node
    {
        if (GameConfig.EnableSinglePlayer) InitSinglePlayerGame();
        if (GameConfig.EnableMultiplayer) InitMultiPlayerGame();

        void InitSinglePlayerGame()
        {
            TPlayer player = null;

            InitSinglePlayerGame();
            Network.StateChanged += InitSinglePlayerGame;

            void InitSinglePlayerGame()
            {
                if (!Network.IsActive)
                    StartSinglePlayerGame();
                else StopSinglePlayerGame();

                void StartSinglePlayerGame()
                {
                    player = Utils.New<TPlayer>();
                    (world ?? this).AddChild(player);
                    InitPlayerColor(player, PlayerData);

                    InitGame?.Invoke(player);
                }

                void StopSinglePlayerGame()
                {
                    player?.DetachChild(free: true);
                    player = null;
                }
            }
        }

        void InitMultiPlayerGame()
        {
            Network.Initialise(world, Utils.New<TPlayer>, OnPlayerAdded, OnPlayerRemoved);

            void OnPlayerAdded(int pid, Node player)
            {
                UI.AddPlayer(pid, out var data);
                InitPlayerColor((TPlayer)player, data);

                if (player.IsLocal())
                    InitGame?.Invoke((TPlayer)player);
            }

            void OnPlayerRemoved(int pid, Node _)
                => UI.RemovePlayer(pid);
        }

        void InitPlayerColor(TPlayer player, IPlayerData data)
        {
            if (GameConfig.EnablePlayerColor)
            {
                SetColor(data.PlayerColor);
                if (player.IsInsideTree()) data.ColorChanged += SetColor;
                player.TreeEntered += () => data.ColorChanged += SetColor;
                player.TreeExiting += () => data.ColorChanged -= SetColor;

                void SetColor(Color color)
                    => SetPlayerColor(player, color);
            }
        }
    }

    #region Godot

    protected virtual void OnReady() { }
    public sealed override void _Ready()
    {
        Editor.Disable(this);
        if (Editor.IsEditor) return;

        ParseCmdLine();
        InitGame();
        OnReady();

        void ParseCmdLine()
            => CmdLine.Parse(Network);

        void InitGame()
        {
            var cfg = GameConfig;

            if (cfg.EnableQuitGame) EnableQuitGame();
            if (cfg.EnableQuickHelp) EnableQuickHelp();
            if (cfg.EnableGameOptions) EnableGameOptions();

            if (cfg.EnableMultiplayer) EnableMultiplayer();

            if (cfg.EnablePlayerName) EnablePlayerName();
            if (cfg.EnablePlayerColor) EnablePlayerColor();
            if (cfg.EnablePlayerAvatars) EnablePlayerAvatars();

            void EnableQuitGame()
                => UI.MainMenu.EnableQuitGame();

            void EnableQuickHelp()
            {
                UI.MainMenu.EnableGameOptions(
                    ("Help", null),
                    ("Quick Help...", UI.QuickHelp.Show));
                UI.QuickHelp.TextFile = cfg.QuickHelp;
            }

            void EnableGameOptions()
            {
                UI.MainMenu.EnableGameOptions(cfg.GameOptions());
                UI.MainMenu.EnableGameOptions(cfg.AddGameOptions);
            }

            void EnableMultiplayer()
            {
                UI.MainMenu.EnableNetworkMenus(Network);
                UI.PlayerList.Initialise(cfg.PlayerData);
            }

            void EnablePlayerName()
                => UI.MainMenu.EnablePlayerName(PlayerData);

            void EnablePlayerColor()
                => UI.MainMenu.EnablePlayerColor(PlayerData);

            void EnablePlayerAvatars()
            {
                UI.MainMenu.EnablePlayerAvatars(PlayerData, cfg.AvatarIcons());
                UI.MainMenu.EnablePlayerAvatars(PlayerData, cfg.PlayerAvatars);
            }
        }
    }

    #endregion
}
