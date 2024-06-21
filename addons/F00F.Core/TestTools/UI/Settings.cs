using System;
using Godot;

namespace F00F;

[Tool]
public partial class Settings : DataViewEx
{
    public Settings()
        => Visible = Editor.IsEditor;

    public void Show(bool show = true)
        => Visible = show && Grid.GetChildCount() > 0;

    public void SetData<T>(string name, T data, Action<T> SetData) where T : Resource, IEditable<T>, new()
    {
        Visible = true;
        Grid.Columns = 2;
        Grid.RemoveChildren();
        Buttons.RemoveChildren();
        Add(Grid, name, data, SetData);
    }

    public void AddGroup<T>(string name, T data, Action<T> SetData) where T : Resource, IEditable<T>, new()
    {
        if (Grid.Columns is 2)
            Clear();

        Visible = true;
        Grid.Columns = 1;
        Grid.AddChild(NewGroup());

        FoldableContainer NewGroup()
        {
            var grp = GroupContainer();
            var grid = GridContainer();
            Add(grid, name, data, SetData);
            grp.AddChild(grid);
            return grp;

            FoldableContainer GroupContainer() => new()
            {
                Name = name,
                Title = name.Capitalise(),
                FoldableGroup = FoldableGroup,
                Folded = name != CurrentGroup,
            };

            GridContainer GridContainer() => new()
            {
                Name = "Content",
                Columns = 2,
            };
        }
    }

    public void RemoveGroup(string name)
    {
        Grid.GetNodeOrNull(name)?.DetachChild(free: true);
        Buttons.GetNodeOrNull($"Reset{name}")?.DetachChild(free: true);
        if (Grid.GetChildCount() is 0 && Buttons.GetChildCount() is 0) Visible = false;
    }

    public bool HasGroup(string name)
        => Grid.GetNodeOrNull(name) is not null;

    public void ToggleGroup<T>(string name, T data, Action<T> SetData) where T : Resource, IEditable<T>, new()
    {
        if (HasGroup(name)) RemoveGroup(name);
        else AddGroup(name, data, SetData);
    }

    public void Clear()
    {
        Visible = false;
        Grid.RemoveChildren();
        Buttons.RemoveChildren();
    }

    #region Private

    private void Add<T>(GridContainer grid, string name, T data, Action<T> SetData) where T : Resource, IEditable<T>, new()
    {
        grid.Init(data.GetEditControls(out var SetEdit));

        var button = UI.NewButton($"Reset{name}");
        Buttons.AddChild(button);
        button.Pressed += () =>
        {
            var data = new T();
            SetData(data);
            SetEdit(data);
        };
    }

    private string CurrentGroup { get; set; }
    private FoldableGroup FoldableGroup => field ??= NewFoldableGroup();

    private FoldableGroup NewFoldableGroup()
    {
        var grp = new FoldableGroup { AllowFoldingAll = false };
        grp.Expanded += OnExpand;
        return grp;

        void OnExpand(FoldableContainer container)
            => CurrentGroup = container.Name;
    }

    #endregion
}
