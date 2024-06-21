using Godot;

namespace F00F;

[Tool]
public partial class RootPopup : CanvasLayer
{
    private Root Root => field ??= GetNode<Root>("Root");

    [Export] public bool ShowOnReady { get; set; } = false;
    [Export] public bool ClickToClose { get; set; } = true;
    [Export] public bool CloseOnCancel { get; set; } = true;

    public RootPopup()
    {
        Layer = Const.CanvasLayer.Popup;
        Visible = Editor.IsEditor;
    }

    protected virtual void OnReady() { }
    public sealed override void _Ready()
    {
        Init();
        OnReady();

        void Init()
        {
            if (ShowOnReady) Show();
            SetProcessInput(Root.VisibleInTree);
            Root.VisibleInTreeChanged += () => SetProcessInput(Root.VisibleInTree);
        }
    }

    protected virtual void OnInput(InputEvent e) { }
    public sealed override void _Input(InputEvent e)
    {
        GD.Print(e);

        OnInput(e);
        HideOnExit();
        ConsumeAllInput();

        void ConsumeAllInput()
        {
            if (Root.MouseNotOver)
                GetViewport().SetInputAsHandled();
        }

        void HideOnExit()
        {
            if (CancelClicked() || CloseClicked()) Hide();

            bool CancelClicked()
                => CloseOnCancel && e.IsActionPressed("ui_cancel");

            bool CloseClicked()
            {
                return ClickToClose
                    && Root.MouseNotOver
                    && e is InputEventMouseButton mouse
                    && mouse.ButtonMask.HasFlag(MouseButtonMask.Left);
            }
        }
    }
}
