using System;
using System.Diagnostics;
using Godot;

namespace F00F;

public enum Exclusivity
{
    Never,
    OnVisible,
    OnMouseOver,
}

[Tool]
public partial class Root : MarginContainer
{
    #region Private

    private MarginContainer OuterMargin => this;
    private MarginContainer InnerMargin => field ??= (MarginContainer)GetNode("%Margin");

    #endregion

    #region Export

    [Export] private Exclusivity Exclusivity { get; set; }
    [Export] private int Margin { get; set => this.Set(ref field, value.ClampMin(0), SetMargins); } = 5;

    #endregion

    public event Action MouseOverChanged;
    public bool MouseOver { get; private set => this.Set(ref field, value, MouseOverChanged); }
    public bool MouseNotOver => !MouseOver;

    public event Action VisibleInTreeChanged;
    public bool VisibleInTree { get; private set => this.Set(ref field, value, VisibleInTreeChanged); }
    public bool NotVisibleInTree => !VisibleInTree;

    private static event Action ExclusivityLockChanged;
    private static Root ExclusivityLock { get; set { if (field != value) { field = value; ExclusivityLockChanged?.Invoke(); } } }
    private bool HasExclusivityPass() => ExclusivityLock is null || ExclusivityLock == this;

    #region Godot

    public sealed override void _EnterTree()
        => ExclusivityLockChanged += SetMouseOver;

    public sealed override void _ExitTree()
        => ExclusivityLockChanged -= SetMouseOver;

    protected virtual void OnReady() { }
    public sealed override void _Ready()
    {
        Init();
        OnReady();

        void Init()
        {
            SetMargins();

            Editor.Disable(this);
            if (Editor.IsEditor) return;

            SetMouseOver();
            InitExclusivity();
            InitVisibleInTree();

            void InitExclusivity()
            {
                switch (Exclusivity)
                {
                    case Exclusivity.OnVisible:
                        VisibleInTreeChanged += () => SetExclusivityLock(VisibleInTree);
                        if (VisibleInTree) SetExclusivityLock(true);
                        break;
                    case Exclusivity.OnMouseOver:
                        MouseOverChanged += () => MyInput.AddActiveItem(MouseOver);
                        if (MouseOver) MyInput.AddActiveItem(true);
                        break;
                }

                OnInputActiveChanged();
                MyInput.ActiveChanged += OnInputActiveChanged;

                void OnInputActiveChanged()
                {
                    var enable = HasExclusivityPass();
                    this.ForEachChild<Control>(x =>
                    {
                        x.Set(OptionButton.PropertyName.Disabled, !enable);
                        x.Set(LineEdit.PropertyName.Editable, enable);
                        x.Set(SpinBox.PropertyName.Editable, enable);
                    });
                }
            }

            void InitVisibleInTree()
            {
                VisibleInTree = IsVisibleInTree();
                VisibilityChanged += () => VisibleInTree = IsVisibleInTree();
            }
        }
    }

    protected virtual void OnInput(InputEvent e) { }
    public sealed override void _Input(InputEvent e)
    {
        Update();
        OnInput(e);

        void Update()
        {
            if (e is InputEventMouseMotion)
                SetMouseOver();
        }
    }

    #endregion

    #region Private

    private void SetMargins()
    {
        OuterMargin.SetMargin(Margin);
        InnerMargin.SetMargin(Margin);
    }

    private void SetExclusivityLock(bool active)
    {
        if (active)
        {
            Debug.Assert(ExclusivityLock is null);
            MyInput.AddActiveItem(true);
            ExclusivityLock = this;
        }
        else
        {
            Debug.Assert(ExclusivityLock == this);
            MyInput.AddActiveItem(false);
            ExclusivityLock = null;
        }
    }

    private void SetMouseOver()
        => MouseOver = HasExclusivityPass() && this.MouseOver();

    #endregion
}
