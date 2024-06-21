using Godot;

namespace F00F;

public partial class Game : Node
{
    protected GUI UI => field ??= GetNode<GUI>("UI");
    protected Network Network => field ??= GetNode<Network>("Network");

    protected virtual IPlayerData PlayerData => UI.PlayerData;
    protected virtual IGameConfig GameConfig => field ??= new GameConfig();

    protected virtual Node CreatePlayer() => new();
    protected virtual void InitPlayer(Node player) { }

    protected virtual void OnReady() { }
    public sealed override void _Ready()
    {
        Editor.Disable(this);
        if (Editor.IsEditor) return;

        ParseCmdLine();
        InitGame();
        OnReady();

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

                Network.Initialise(CreatePlayer, OnPlayerAdded, OnPlayerRemoved);

                void OnPlayerAdded(int pid, Node player)
                {
                    UI.AddPlayer(pid);

                    if (player.IsLocal())
                        InitPlayer(player);
                }

                void OnPlayerRemoved(int pid, Node _)
                    => UI.RemovePlayer(pid);
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

        void ParseCmdLine()
            => CmdLine.Parse(Network);
    }
}
