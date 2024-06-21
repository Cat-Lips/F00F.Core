using System;
using Godot;

namespace F00F;

[Tool]
public partial class Game : Node
{
    #region Private

    protected GUI UI => field ??= (GUI)GetNode("UI");
    protected Network Network => field ??= (Network)GetNode("Network");

    #endregion

    protected virtual IPlayerData PlayerData => UI.PlayerData;
    protected virtual IGameConfig GameConfig => field ??= new GameConfig();

    protected void InitGame<TPlayer>(
        Node world = null,
        Action<TPlayer> InitGame = null,
        Action<TPlayer, Color> SetPlayerColor = null) where TPlayer : Node
    {
        if (GameConfig.EnableSinglePlayer) InitSinglePlayerGame();
        if (GameConfig.EnableMultiplayer) InitMultiplayerGame();

        void InitSinglePlayerGame()
        {
            TPlayer player = null;
            UI.InitGame(Network, OnStartGame, OnEndGame);

            void OnStartGame()
            {
                player = Utils.New<TPlayer>();
                (world ?? this).AddChild(player);
                InitPlayerColor(player, PlayerData);

                InitGame?.Invoke(player);
            }

            void OnEndGame()
            {
                player?.DetachChild(free: true);
                player = null;
            }
        }

        void InitMultiplayerGame()
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
                data.ColorChanged += SetColor;

                void SetColor(Color color)
                    => SetPlayerColor?.Invoke(player, color);
            }
        }
    }

    #region Godot

    protected virtual void OnReady() { }
    protected virtual void InitInput() { }
    public sealed override void _Ready()
    {
        Editor.Disable(this);
        if (Editor.IsEditor) return;

        ParseCmdLine();
        InitInput();
        InitGame();
        OnReady();

        void ParseCmdLine()
            => CmdLine.Parse(Network);

        void InitGame()
            => UI.Initialise(GameConfig, Network);
    }

    protected virtual bool OnUnhandledKeyInput(InputEventKey e) => false;
    public sealed override void _UnhandledKeyInput(InputEvent e)
    {
        if (e is InputEventKey key)
        {
            if (OnUnhandledKeyInput(key)) return;
            if (this.Handle(key.JustPressed(GameConfig.QuickExitKey), QuickExit)) return;
            if (this.Handle(key.JustPressed(GameConfig.QuickHelpKey), QuickHelp)) return;

            void QuickExit()
                => App.Quit(this);

            void QuickHelp()
                => UI.QuickHelp.Visible = !UI.QuickHelp.Visible;
        }
    }

    #endregion
}
