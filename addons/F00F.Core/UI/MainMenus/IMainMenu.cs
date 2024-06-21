using System;
using System.Collections.Generic;
using Godot;

namespace F00F;

public interface IMainMenu
{
    void EnableNetworkMenus(Network network);

    void EnablePlayerName(IPlayerData data);
    void EnablePlayerColor(IPlayerData data);
    void EnablePlayerAvatars(IPlayerData data, params string[] avatars);
    void EnablePlayerAvatars(IPlayerData data, params (string Key, Texture2D Icon)[] avatars);

    // MainMenu1
    void EnableGameOptions(Action<PopupMenu> AddItems) { }
    void EnableGameOptions(params (string Name, Action Action)[] items) { }

    // MainMenu2
    void EnableGameOptions(params IEnumerable<(string Label, Control Control)> items) { }
    void EnableGameOptions(string group, params IEnumerable<(string Label, Control Control)> items) { }

    void EnableQuickHelp(RootPopup help);
    void EnableQuitGame();

    void InitGame(Network network, Action StartGame, Action EndGame)
    {
        InitGame();
        network.StateChanged += InitGame;

        void InitGame()
        {
            if (!network.IsActive)
                StartGame();
            else EndGame();
        }
    }
}
