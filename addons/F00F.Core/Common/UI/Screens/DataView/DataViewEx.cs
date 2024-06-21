using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace F00F;

[Tool]
public partial class DataViewEx : DataView
{
    protected Label Title => field ??= (Label)GetNode("%Title");
    protected Control Buttons => field ??= (Control)GetNode("%Buttons");
    public string TitleText { get => Title.Text; set => Title.Text = value; }

    #region Xtras

    public event Action ShowXtrasChanged;
    public bool ShowXtras { get; set => this.Set(ref field, value, OnShowXtrasChanged, ShowXtrasChanged); } = true;

    public event Action ShowXtrasOnGridActiveChanged;
    public bool ShowXtrasOnGridActive { get; set => this.Set(ref field, value, OnShowXtrasOnGridActiveChanged, ShowXtrasOnGridActiveChanged); } = true;

    protected IEnumerable<Control> AllXtras => field ??= Layout.GetChildren<Control>();
    protected IEnumerable<Control> GridXtras => field ??= AllXtras.Except([Buttons]);
    protected virtual Control[] Xtras => field ??= [.. GridXtras];

    #region Private

    private void OnShowXtrasChanged()
        => Xtras.ForEach(x => x.Visible = ShowXtras);

    private void OnShowXtrasOnGridActiveChanged(bool active)
        => ShowXtras = !active || GridActive;

    #endregion

    #endregion

    #region GridActive

    public event Action GridActiveChanged;
    public bool GridActive { get; private set => this.Set(ref field, value, OnGridActiveChanged, GridActiveChanged); }

    #region Private

    private void OnGridActiveChanged(bool active)
    {
        if (ShowXtrasOnGridActive)
            ShowXtras = active;
    }

    #endregion

    #endregion

    #region GridMonitor

    public event Action GridMonitorChanged;
    public bool GridMonitor { get; set => this.Set(ref field, value, OnGridMonitorChanged, GridMonitorChanged); }

    #region Private

    private void OnGridMonitorChanged(bool active)
    {
        ChildEnteredTree -= GridMonitor_OnChildEntered;
        ChildExitingTree -= GridMonitor_OnChildExiting;

        if (active)
        {
            GridMonitor_Evaluate();
            ChildEnteredTree += GridMonitor_OnChildEntered;
            ChildExitingTree += GridMonitor_OnChildExiting;
        }
    }

    private void GridMonitor_OnChildEntered(Node _)
        => GridMonitor_Evaluate();

    private void GridMonitor_OnChildExiting(Node _)
        => Callable.From(GridMonitor_Evaluate).CallDeferred();

    private void GridMonitor_Evaluate()
        => GridActive = GetChildCount() > 0;

    #endregion

    #endregion
}
