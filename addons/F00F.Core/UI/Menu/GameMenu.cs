using System;
using Godot;

namespace F00F;

[Tool]
public partial class GameMenu : SlideMenu
{
    private Button QuitGame => field ??= GetNode<Button>("%QuitGame");
    private MenuButton GameOptions => field ??= GetNode<MenuButton>("%GameOptions");
    private Separator Sep => field ??= GetNode<Separator>("%Sep");

    public GameMenu()
        => Visible = Editor.IsEditor;

    protected sealed override void OnReady1()
    {
        this.InitQuit();

        QuitGame.Visible = Editor.IsEditor;
        GameOptions.Visible = Editor.IsEditor;

        SetSepVisibility();
        QuitGame.VisibilityChanged += SetSepVisibility;
        GameOptions.VisibilityChanged += SetSepVisibility;

        void SetSepVisibility()
            => Sep.Visible = QuitGame.Visible && GameOptions.Visible;
    }

    public void EnableQuitGame()
    {
        Visible = true;
        QuitGame.Visible = true;
        QuitGame.Pressed += OnQuitGame;

        void OnQuitGame()
            => this.GetMainWindow().PropagateNotification((int)NotificationWMCloseRequest);
    }

    public void EnableGameOptions(Action<PopupMenu> AddItems)
    {
        AddItems(GameOptions.GetPopup());
        if (GameOptions.ItemCount is 0) return;
        GameOptions.Visible = true;
        Visible = true;
    }

    public void EnableGameOptions(params (string Label, Action Action)[] items)
    {
        if (items.Empty()) return;
        GameOptions.AddItems(items);
        GameOptions.Visible = true;
        Visible = true;
    }

    public sealed override void _Notification(int what)
        => this.NotifyQuit(what);
}
