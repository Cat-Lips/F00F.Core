using System;
using Godot;

namespace F00F;

public partial class Settings : DataViewEx
{
    public Settings()
        => Visible = Editor.IsEditor;

    public void Clear()
    {
        Visible = false;
        Grid.RemoveChildren();
        Buttons.RemoveChildren();
    }

    public void Show(bool show = true)
        => Visible = show && Grid.GetChildCount() > 0;

    public void SetData<T>(string name, T data, Action<T> SetData, bool show = true) where T : Resource, IEditable<T>, new()
    {
        Visible = show;
        Grid.Columns = 2;
        Grid.RemoveChildren();
        Buttons.RemoveChildren();
        Add(Grid, name, data, SetData);
    }
#if GODOT4_5_OR_GREATER
    private FoldableGroup AccordianGroup { get; } = new();
    public void AddGroup<T>(string name, T data, Action<T> SetData, bool show = true) where T : Resource, IEditable<T>, new()
    {
        Visible = show;
        Grid.Columns = 1;
        Grid.AddChild(NewGroup());

        FoldableContainer NewGroup()
        {
            var grp = GroupContainer();
            var grid = GridContainer();
            Add(grid, name, data, SetData);
            grp.AddChild(grid, forceReadableName: true);
            return grp;

            FoldableContainer GroupContainer() => new()
            {
                Name = $"{name}Group",
                Text = name.Capitalise(),
                FoldableGroup = AccordianGroup,
            };

            GridContainer GridContainer() => new()
            {
                Name = $"{name}Grid",
                Columns = 2,
            };
        }
    }
#endif
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
}
