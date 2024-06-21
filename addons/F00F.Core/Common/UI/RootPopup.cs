using Godot;

namespace F00F;

[Tool]
public partial class RootPopup : CanvasLayer
{
    private Root Root => field ??= (Root)GetNode("Root");

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
        ShowOnReady();

        void Init()
        {
            InitMouse();
            InitProcess();

            void InitMouse()
            {
                Root.VisibleInTreeChanged += () =>
                {
                    if (Root.VisibleInTree)
                        MyInput.ShowMouse = true;
                };
            }

            //void InitMouse()
            //{
            //    var show = MyInput.ShowMouse;
            //    Root.VisibleInTreeChanged += () =>
            //    {
            //        if (Root.VisibleInTree)
            //        {
            //            show = MyInput.ShowMouse;
            //            MyInput.ShowMouse = true;
            //        }
            //        else
            //        {
            //            MyInput.ShowMouse = show;
            //        }
            //    };
            //}

            void InitProcess()
            {
                SetProcessInput(Root.VisibleInTree);
                Root.VisibleInTreeChanged += () => SetProcessInput(Root.VisibleInTree);
            }
        }

        void ShowOnReady()
        {
            if (this.ShowOnReady)
                Show();
        }
    }

    protected virtual void OnInput(InputEvent e) { }
    public sealed override void _Input(InputEvent e)
    {
        OnInput(e);
        HideOnExit();
        ConsumeAllInput();

        void ConsumeAllInput()
        {
            if (Root.MouseNotOver)
                this.Handled();
        }

        void HideOnExit()
        {
            if (CancelClicked() || CloseClicked())
            {
                Hide();
                this.Handled();
            }

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
