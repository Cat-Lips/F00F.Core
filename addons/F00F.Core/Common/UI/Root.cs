using System;
using System.Diagnostics;
using Godot;

namespace F00F;

[Tool]
public partial class Root : MarginContainer
{
    #region Private

    private MarginContainer OuterMargin => this;
    private MarginContainer InnerMargin => field ??= (MarginContainer)GetNode("Panel/Margin");

    #endregion

    #region Export

    public event Action MarginChanged;
    public event Action ExclusivityChanged;

    public enum ExclusivityType { Never, OnVisible, OnMouseOver }

    [Export] private int Margin { get; set => this.Set(ref field, value.ClampMin(0), MarginChanged); } = 5;
    [Export] private ExclusivityType Exclusivity { get; set => this.Set(ref field, value, ExclusivityChanged); }

    #endregion

    public event Action MouseOverChanged;
    public bool MouseOver { get; private set => this.Set(ref field, value, MouseOverChanged); }
    public bool MouseNotOver => !MouseOver;

    public event Action VisibleInTreeChanged;
    public bool VisibleInTree { get; private set => this.Set(ref field, value, VisibleInTreeChanged); }
    public bool NotVisibleInTree => !VisibleInTree;

    private static event Action ExclusivityLockChanged;
    private static Root ExclusivityLock { get; set => SetX.Set(ref field, value, ExclusivityLockChanged); }
    private bool HasExclusivityPass() => ExclusivityLock is null || ExclusivityLock == this;

    #region Godot

    protected virtual void OnReady() { }
    public sealed override void _Ready()
    {
        Init();
        OnReady();

        void Init()
        {
            InitMargins();

            Editor.Disable(this);
            if (Editor.IsEditor) return;

            InitMouseOver();
            InitExclusivity();
            InitVisibleInTree();
            InitInputActivation();
        }
    }

    #endregion

    #region Private

    private void InitMargins()
    {
        OnMarginChanged();
        MarginChanged += OnMarginChanged;

        void OnMarginChanged()
        {
            OuterMargin.SetMargin(Margin);
            InnerMargin.SetMargin(Margin);
        }
    }

    private void InitMouseOver()
    {
        this.InitMouseOver(OnMouseOver);
        this.SafeInit(InitExclusivity, DnitExclusivity);

        void InitExclusivity()
            => ExclusivityLockChanged += SetMouseOver;

        void DnitExclusivity()
            => ExclusivityLockChanged -= SetMouseOver;

        void OnMouseOver(bool MouseOver)
            => this.MouseOver = HasExclusivityPass() && MouseOver;

        void SetMouseOver()
            => OnMouseOver(this.MouseOver());
    }

    private void InitExclusivity()
    {
        InitMouseOverExclusivity();
        InitVisibilityExclusivity();
        MouseOverChanged += SetMouseOverExclusivity;
        VisibleInTreeChanged += SetVisibilityExclusivity;

        void InitMouseOverExclusivity()
        {
            if (Exclusivity is ExclusivityType.OnMouseOver)
                if (MouseOver) MyInput.Instance.AddActiveItem(true);
        }

        void SetMouseOverExclusivity()
        {
            if (Exclusivity is ExclusivityType.OnMouseOver)
                MyInput.Instance.AddActiveItem(MouseOver);
        }

        void InitVisibilityExclusivity()
        {
            if (Exclusivity is ExclusivityType.OnVisible)
                if (VisibleInTree) SetExclusivityLock(true);
        }

        void SetVisibilityExclusivity()
        {
            if (Exclusivity is ExclusivityType.OnVisible)
                SetExclusivityLock(VisibleInTree);
        }

        void SetExclusivityLock(bool active)
        {
            if (active)
            {
                Debug.Assert(ExclusivityLock is null);
                MyInput.Instance.AddActiveItem(true);
                ExclusivityLock = this;
            }
            else
            {
                Debug.Assert(ExclusivityLock == this);
                MyInput.Instance.AddActiveItem(false);
                ExclusivityLock = null;
            }
        }
    }

    private void InitVisibleInTree()
    {
        VisibleInTree = IsVisibleInTree();
        VisibilityChanged += () => VisibleInTree = IsVisibleInTree();
    }

    private void InitInputActivation()
    {
        OnInputActiveChanged();
        MyInput.Instance.ActiveChanged += OnInputActiveChanged;

        void OnInputActiveChanged()
        {
            if (HasExclusivityPass())
                this.DeepRestore();
            else this.DeepDisable();
        }
    }

    #endregion
}
