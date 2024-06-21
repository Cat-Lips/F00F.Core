using System;
using Godot;

namespace F00F;

[Tool]
public partial class Settings : DataViewEx
{
    #region Instance

    public static Settings Instance { get => field.ValidOrNull(); private set; }

    public Settings()
    {
        Instance ??= this;
        Visible = Editor.IsEditor;
    }

    #endregion

    public void Show(bool show = true)
        => Visible = show && Grid.GetChildCount() > 0;

    #region SetData

    public void SetData<T>(string name, T data, Action<T> Set) where T : Resource, IEditable<T>, new()
        => SetData(name, data, Set, () => new T());

    public void SetData<T>(string name, T data, Action<T> Set, Func<T> Reset) where T : Resource, IEditable<T>
        => SetData(name, (IEditable)data, x => Set((T)x), Reset);

    public void SetData(string name, IEditable data, Action<IEditable> Set, Func<IEditable> Reset = null)
    {
        Visible = true;
        Grid.Columns = 2;
        Grid.RemoveChildren();
        Buttons.RemoveChildren();
        Add(Grid, name, data, Set, Reset);
    }

    #endregion

    #region AddGroup

    public void AddGroup<T>(string name, T data, Action<T> Set) where T : Resource, IEditable<T>, new()
        => AddGroup(name, data, Set, () => new T());

    public void AddGroup<T>(string name, T data, Action<T> Set, Func<T> Reset) where T : Resource, IEditable<T>
        => AddGroup(name, (IEditable)data, x => Set((T)x), Reset);

    public void AddGroup(string name, IEditable data, Action<IEditable> Set, Func<IEditable> Reset = null)
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
            Add(grid, name, data, Set, Reset);
            grp.AddChild(grid);
            return grp;

            FoldableContainer GroupContainer() => new()
            {
                Name = name,
                Title = name.Capitalise(),
                FoldableGroup = FoldableGroup,
                Folded = false,
            };

            GridContainer GridContainer() => new()
            {
                Name = "Content",
                Columns = 2,
            };
        }
    }

    #endregion

    #region ToggleGroup

    public void ToggleGroup<T>(string name, T data, Action<T> Set) where T : Resource, IEditable<T>, new()
        => ToggleGroup(name, data, Set, () => new T());

    public void ToggleGroup<T>(string name, T data, Action<T> Set, Func<T> Reset) where T : Resource, IEditable<T>
        => ToggleGroup(name, (IEditable)data, x => Set((T)x), Reset);

    public void ToggleGroup(string name, IEditable data, Action<IEditable> Set, Func<IEditable> Reset = null)
    {
        if (HasGroup(name)) RemoveGroup(name);
        else AddGroup(name, data, Set, Reset);
    }

    #endregion

    public void RemoveGroup(string name)
    {
        Grid.GetNodeOrNull(name)?.DetachChild(free: true);
        Buttons.GetNodeOrNull($"Reset{name}")?.DetachChild(free: true);
        if (Grid.GetChildCount() is 0 && Buttons.GetChildCount() is 0) Visible = false;
    }

    public bool HasGroup(string name)
        => Grid.GetNodeOrNull(name) is not null;

    public void Clear()
    {
        Visible = false;
        Grid.RemoveChildren();
        Buttons.RemoveChildren();
    }

    #region Private

    private void Add(GridContainer grid, string name, IEditable data, Action<IEditable> SetData, Func<IEditable> NewData = null)
    {
        NewData ??= () => (IEditable)Activator.CreateInstance(data.GetType());
        grid.Init(data.GetEditControls(out var SetEdit));

        var button = UI.NewButton($"Reset{name}");
        Buttons.AddChild(button);
        button.Pressed += () =>
        {
            var x = NewData();
            SetData(x);
            SetEdit((Resource)x);
        };
    }

    private string CurrentGroup { get; set; }
    private FoldableGroup FoldableGroup => field ??= NewFoldableGroup();

    private FoldableGroup NewFoldableGroup()
    {
        var grp = new FoldableGroup();
        grp.Expanded += OnExpand;
        return grp;

        void OnExpand(FoldableContainer container)
            => CurrentGroup = container.Name;
    }

    #endregion
}
