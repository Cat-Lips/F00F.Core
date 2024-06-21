using System;
using System.Collections.Generic;
using Godot;

namespace F00F;

[Tool]
public partial class MainMenu2 : RootPopup, IMainMenu
{
    #region Private

    private MainMenu MainMenu => field ??= (MainMenu)GetNode("%MainMenu");
    private PauseMenu PauseMenu => field ??= (PauseMenu)GetNode("%PauseMenu");
    private EndGameMenu EndGameMenu => field ??= (EndGameMenu)GetNode("%EndGameMenu");
    private StatusBar NetworkStatus => field ??= (StatusBar)GetNode("%NetworkStatus");

    private enum Menu { None, Main, Pause, EndGame }
    private Menu MyMenu { get; set => this.Set(ref field, value, OnMenuSet); }
    private void OnMenuSet()
    {
        switch (MyMenu)
        {
            case Menu.None: HideMenu(); break;
            case Menu.Main: ShowMainMenu(); break;
            case Menu.Pause: ShowPauseMenu(); break;
            case Menu.EndGame: ShowEndGameMenu(); break;
        }

        void HideMenu()
        {
            Hide();
            MainMenu.Visible = false;
            PauseMenu.Visible = false;
            EndGameMenu.Visible = false;

            PauseAction = PauseGame;
        }

        void ShowMainMenu()
        {
            Show();
            MainMenu.Visible = true;
            PauseMenu.Visible = false;
            EndGameMenu.Visible = false;

            PauseAction = null;
        }

        void ShowPauseMenu()
        {
            Show();
            MainMenu.Visible = false;
            PauseMenu.Visible = true;
            EndGameMenu.Visible = false;

            PauseAction = ResumeGame;
        }

        void ShowEndGameMenu()
        {
            Show();
            MainMenu.Visible = false;
            PauseMenu.Visible = false;
            EndGameMenu.Visible = true;

            PauseAction = null;
        }
    }

    private Action PauseAction;

    #endregion

    #region Godot

    protected sealed override void OnReady()
    {
        if (Editor.IsEditor) return;

        InitParts();
        ShowMainMenu();

        void InitParts()
        {
            MainMenu.Start += HideMainMenu;

            PauseMenu.Resume.Pressed += ResumeGame;
            PauseMenu.Quit.Pressed += ShowMainMenu;

            EndGameMenu.Continue.Pressed += ShowMainMenu;
            EndGameMenu.Quit.Pressed += ExitGame;

            ProcessMode = ProcessModeEnum.Always;
        }
    }

    protected sealed override void OnInput(InputEvent e)
    {
        if (PauseAction.NotNull() && this.Handle(e.IsActionPressed(MyInput.Pause), PauseAction)) return;
    }

    public sealed override void _UnhandledKeyInput(InputEvent e)
    {
        if (PauseAction.NotNull() && this.Handle(e.IsActionPressed(MyInput.Pause), PauseAction)) return;
    }

    #endregion

    #region Config

    public MainMenu2()
        => MyInput.ShowWithMouse(this);

    public void EnableNetworkMenus(Network network)
    {
        MainMenu.EnableNetworkMenus(network);
        network.ClientStatus += NetworkStatus.SetStatus;
        network.ServerStatus += NetworkStatus.SetStatus;
        network.StateChanged += OnNetworkStateChanged;

        void OnNetworkStateChanged()
        {
            switch (network.State)
            {
                case NetworkState.NoConnection: ResetStatus(); break;
                case NetworkState.StartingServer: ShowStatus(); break;
                case NetworkState.ServerActive: StartGame(); break;
                case NetworkState.ServerError: SetAction("OK", ShowMainMenu); break;
                case NetworkState.ClientConnecting: SetAction("Cancel", CancelConnect); break;
                case NetworkState.ClientConnected: StartGame(); break;
                case NetworkState.ClientError: SetAction("OK", ShowMainMenu); break;
                case NetworkState.ClientDisconnected: SetAction("OK", ShowMainMenu); break;
            }

            void ShowStatus()
                => NetworkStatus.Visible = true;

            void SetAction(string text, Action action)
            {
                NetworkStatus.Visible = true;
                NetworkStatus.SetAction(text, action);
            }

            void ResetStatus()
            {
                NetworkStatus.ClearAction();
                NetworkStatus.ClearStatus();
                NetworkStatus.Visible = false;
            }

            void CancelConnect()
            {
                network.CloseClient();
                ShowMainMenu();
            }

            void StartGame()
            {

            }
        }
    }

    public void EnablePlayerName(IPlayerData data)
        => MainMenu.EnablePlayerName(data);

    public void EnablePlayerColor(IPlayerData data)
        => MainMenu.EnablePlayerColor(data);

    public void EnablePlayerAvatars(IPlayerData data, params string[] avatars)
        => MainMenu.EnablePlayerAvatars(data, avatars);

    public void EnablePlayerAvatars(IPlayerData data, params (string Key, Texture2D Icon)[] avatars)
        => MainMenu.EnablePlayerAvatars(data, avatars);

    public void EnableGameOptions(params IEnumerable<(string Label, Control Control)> items)
        => MainMenu.EnableGameOptions(items);

    public void EnableGameOptions(string group, params IEnumerable<(string Label, Control Control)> items)
        => MainMenu.EnableGameOptions(group, items);

    public void EnableQuickHelp(RootPopup help)
        => MainMenu.EnableQuickHelp(help);

    public void EnableQuitGame()
        => MainMenu.EnableQuitGame();

    #endregion

    #region Private

    private void ShowMainMenu()
        => MyMenu = Menu.Main;

    private void HideMainMenu()
        => MyMenu = Menu.None;

    private void PauseGame()
    {
        MyMenu = Menu.Pause;
        GetTree().Paused = true;
    }

    private void ResumeGame()
    {
        MyMenu = Menu.None;
        GetTree().Paused = false;
    }

    private void ExitGame()
        => App.Quit(this);

    #endregion
}
