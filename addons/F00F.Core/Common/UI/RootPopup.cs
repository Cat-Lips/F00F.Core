using System.Diagnostics;
using Godot;

namespace F00F;

[Tool]
public partial class RootPopup : CanvasLayer
{
    [Export] public bool ClickToClose { get; set; } = true;

    public RootPopup()
        => Layer = Const.CanvasLayer.RootPopup;

    protected virtual void OnReady() { }
    public sealed override void _Ready()
    {
        InitPopup();
        OnReady();

        void InitPopup()
        {
            SetProcessUnhandledInput(Visible = Editor.IsEditor);
            VisibilityChanged += () => SetProcessUnhandledInput(Visible);
        }
    }

    public sealed override void _UnhandledInput(InputEvent e)
    {
        Debug.Assert(Visible);

        ConsumeAllInput();
        if (CriticalKeyHit()) Hide();

        void ConsumeAllInput()
            => GetViewport().SetInputAsHandled();

        bool CriticalKeyHit()
        {
            return e.IsActionPressed("ui_cancel") ||
                ClickToClose && e is InputEventMouseButton mouse && mouse.ButtonMask.HasFlag(MouseButtonMask.Left);
        }
    }
}
