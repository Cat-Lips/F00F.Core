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

    public event Action<string> PlayerNameChanged;
    public event Action<Color> PlayerColorChanged;
    public event Action<string> PlayerAvatarChanged;

    public string PlayerName { get; private set => this.Set(ref field, value, PlayerNameChanged); }
    public Color PlayerColor { get; private set => this.Set(ref field, value, PlayerColorChanged); }
    public string PlayerAvatar { get; private set => this.Set(ref field, value, PlayerAvatarChanged); }

    #region Config

    public void EnableNetworkMenus(Network network)
    {
        ServerMenu.Initialise(network);
        ClientMenu.Initialise(network);
    }

    public void EnablePlayerName()
        => PlayerMenu.EnablePlayerName(cfg, x => PlayerName = x);

    public void EnablePlayerColor()
        => PlayerMenu.EnablePlayerColor(cfg, x => PlayerColor = x);

    public void EnablePlayerAvatars(params string[] avatars)
        => PlayerMenu.EnablePlayerAvatar(cfg, x => PlayerAvatar = x, avatars);

    public void EnablePlayerAvatars(params (string Key, Texture2D Icon)[] avatars)
        => PlayerMenu.EnablePlayerAvatar(cfg, x => PlayerAvatar = x, avatars);

    public void EnableGameOptions(Action<PopupMenu> AddItems)
        => GameMenu.EnableGameOptions(AddItems);

    public void EnableGameOptions(params (string Name, Action Action)[] items)
        => GameMenu.EnableGameOptions(items);

    public void EnableQuitGame()
        => GameMenu.EnableQuitGame();

    #endregion
}
