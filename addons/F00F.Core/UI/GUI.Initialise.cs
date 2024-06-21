namespace F00F;

public partial class GUI
{
    public void Initialise(IGameConfig cfg, Network network)
    {
        var mm = GetNodeOrNull("MainMenu") as IMainMenu;

        if (cfg.ShowQuickHelp) ShowQuickHelp();

        if (cfg.EnableQuitGame) EnableQuitGame();
        if (cfg.EnableQuickHelp) EnableQuickHelp();
        if (cfg.EnableGameOptions) EnableGameOptions();

        if (cfg.EnableMultiplayer) EnableMultiplayer();

        if (cfg.EnablePlayerName) EnablePlayerName();
        if (cfg.EnablePlayerColor) EnablePlayerColor();
        if (cfg.EnablePlayerAvatars) EnablePlayerAvatars();

        void ShowQuickHelp()
        {
            QuickHelp.Show();
            QuickHelp.TextFile = cfg.QuickHelpFile;
        }

        void EnableQuitGame()
            => mm?.EnableQuitGame();

        void EnableQuickHelp()
        {
            mm?.EnableQuickHelp(QuickHelp);
            QuickHelp.TextFile = cfg.QuickHelpFile;
        }

        void EnableGameOptions()
        {
            // MainMenu1
            mm?.EnableGameOptions(cfg.GameOptions());
            mm?.EnableGameOptions(cfg.AddGameOptions);

            // MainMenu2
            mm?.EnableGameOptions(cfg.GetGameOptions());
            cfg.GameOptionGroups?.ForEach(grp => mm?.EnableGameOptions(cfg.GetGameOptions(grp)));
        }

        void EnableMultiplayer()
        {
            mm?.EnableNetworkMenus(network);
            PlayerList.Initialise(cfg.PlayerData);
        }

        void EnablePlayerName()
            => mm?.EnablePlayerName(PlayerData);

        void EnablePlayerColor()
            => mm?.EnablePlayerColor(PlayerData);

        void EnablePlayerAvatars()
        {
            mm?.EnablePlayerAvatars(PlayerData, cfg.AvatarIcons());
            mm?.EnablePlayerAvatars(PlayerData, cfg.PlayerAvatars);
        }
    }
}
