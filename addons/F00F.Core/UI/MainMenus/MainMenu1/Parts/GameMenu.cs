using System;
using Godot;

namespace F00F;

[Tool]
public partial class GameMenu : SlideMenu
{
    private Button QuitGame => field ??= (Button)GetNode("%QuitGame");
    private MenuButton GameOptions => field ??= (MenuButton)GetNode("%GameOptions");
    private Separator Sep => field ??= (Separator)GetNode("%Sep");

    public GameMenu()
        => Visible = Editor.IsEditor;

    protected sealed override void OnReady1()
    {
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
            => App.Quit(this);
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
        if (items.IsNullOrEmpty()) return;
        GameOptions.AddItems(items);
        GameOptions.Visible = true;
        Visible = true;
    }
}
