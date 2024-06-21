using System;
using Godot;

namespace F00F;

public partial class Settings : DataViewEx
{
    public Settings()
    {
        Title = "Settings";
        Visible = Editor.IsEditor;
    }

    public void Clear()
    {
        Visible = false;
        Grid.RemoveChildren();
        Buttons.RemoveChildren();
    }

    public void Add<T>(string name, T data, Action<T> SetData) where T : Resource, IEditable<T>, new()
    {
        if (data is null) return;

        Visible = true;
        Grid.Init(data.GetEditControls(out var SetEdit));

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
