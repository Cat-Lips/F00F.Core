using System;
using Godot;

namespace F00F;

[Tool]
public partial class MainMenu1 : Root, IMainMenu
{
    #region Private

    private readonly Config cfg = new("MainMenu");

    private ServerMenu ServerMenu => field ??= (ServerMenu)GetNode("%ServerMenu");
    private ClientMenu ClientMenu => field ??= (ClientMenu)GetNode("%ClientMenu");
    private PlayerMenu PlayerMenu => field ??= (PlayerMenu)GetNode("%PlayerMenu");
    private GameMenu GameMenu => field ??= (GameMenu)GetNode("%GameMenu");

    #endregion

    #region Config

    public MainMenu1()
        => MyInput.ShowWithMouse(this);

    public void EnableNetworkMenus(Network network)
    {
        ServerMenu.Initialise(network);
        ClientMenu.Initialise(network);
    }

    public void EnablePlayerName(IPlayerData data)
        => PlayerMenu.EnablePlayerName(cfg, x => data.PlayerName = x);

    public void EnablePlayerColor(IPlayerData data)
        => PlayerMenu.EnablePlayerColor(cfg, x => data.PlayerColor = x);

    public void EnablePlayerAvatars(IPlayerData data, params string[] avatars)
        => PlayerMenu.EnablePlayerAvatar(cfg, x => data.PlayerAvatar = x, avatars);

    public void EnablePlayerAvatars(IPlayerData data, params (string Key, Texture2D Icon)[] avatars)
        => PlayerMenu.EnablePlayerAvatar(cfg, x => data.PlayerAvatar = x, avatars);

    public void EnableGameOptions(Action<PopupMenu> AddItems)
        => GameMenu.EnableGameOptions(AddItems);

    public void EnableGameOptions(params (string Name, Action Action)[] items)
        => GameMenu.EnableGameOptions(items);

    public void EnableQuickHelp(RootPopup help)
    {
        EnableGameOptions(
            ("Help", null),
            ("Quick Help...", help.Show));
    }

    public void EnableQuitGame()
        => GameMenu.EnableQuitGame();

    #endregion
}
