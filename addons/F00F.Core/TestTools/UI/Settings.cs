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

    public void SetData<T>(string name, T data, Action<T> SetData) where T : Resource, IEditable<T>, new()
    {
        Visible = true;
        Grid.Columns = 2;
        Grid.RemoveChildren();
        Buttons.RemoveChildren();
        Add(Grid, name, data, SetData);
    }

#if GODOT4_5_OR_GREATER

    public void AddGroup<T>(string name, T data, Action<T> SetData) where T : Resource, IEditable<T>, new()
    {
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

    #region Private

    private string CurrentGroup { get; set; }
    private FoldableGroup FoldableGroup => field ??= InitFoldableGroup();

    private FoldableGroup InitFoldableGroup()
    {
        var grp = new FoldableGroup { AllowFoldingAll = false };
        grp.Expanded += OnExpand;
        return grp;

        void OnExpand(FoldableContainer container)
            => CurrentGroup = container.Name;
    }

    #endregion

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
