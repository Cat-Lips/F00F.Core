using System.Collections.Generic;
using Godot;

namespace F00F;

[Tool]
public partial class MainMenu2 : RootPopup, IMainMenu
{
    private MainMenu2_Layout Layout => field ??= GetNode<MainMenu2_Layout>("Layout");

    public void EnableNetworkMenus(Network network)
        => Layout.EnableNetworkMenus(network);

    public void EnablePlayerName(IPlayerData data)
        => Layout.EnablePlayerName(data);

    public void EnablePlayerColor(IPlayerData data)
        => Layout.EnablePlayerColor(data);

    public void EnablePlayerAvatars(IPlayerData data, params string[] avatars)
        => Layout.EnablePlayerAvatars(data, avatars);

    public void EnablePlayerAvatars(IPlayerData data, params (string Key, Texture2D Icon)[] avatars)
        => Layout.EnablePlayerAvatars(data, avatars);

    public void EnableGameOptions(params IEnumerable<(string Label, Control Control)> items)
        => Layout.EnableGameOptions(items);

    public void EnableGameOptions(string group, params IEnumerable<(string Label, Control Control)> items)
        => Layout.EnableGameOptions(group, items);

    public void EnableQuickHelp(RootPopup help)
        => Layout.EnableQuickHelp(help);

    public void EnableQuitGame()
        => Layout.EnableQuitGame();
}
