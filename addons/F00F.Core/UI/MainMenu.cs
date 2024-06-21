using System;
using Godot;

namespace F00F;

[Tool]
public partial class MainMenu : Root
{
    #region Private

    private readonly Config<MainMenu> cfg = new();

    private ServerMenu ServerMenu => field ??= GetNode<ServerMenu>("%ServerMenu");
    private ClientMenu ClientMenu => field ??= GetNode<ClientMenu>("%ClientMenu");
    private PlayerMenu PlayerMenu => field ??= GetNode<PlayerMenu>("%PlayerMenu");
    private GameMenu GameMenu => field ??= GetNode<GameMenu>("%GameMenu");

    #endregion

    #region Config

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

    public void EnableQuitGame()
        => GameMenu.EnableQuitGame();

    #endregion
}
