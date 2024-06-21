using System.Collections.Generic;
using Godot;

namespace F00F;

using ControlPair = (Control Label, Control EditControl);

[Tool]
public partial class DataView : Root
{
    public GridContainer Grid => field ??= (GridContainer)GetNode("%Grid");

    public void Add(IEnumerable<ControlPair> content)
        => Grid.Init(content);

    public void Sep() => Grid.AddRowSeparator();
    public void AddSep() => Grid.AddRowSeparator();
    public void AddRowSeparator() => Grid.AddRowSeparator();

    public void Add(Control value) => Add(value.Name, value);
    public void Add(string name, Control value)
    {
        Grid.AddChild(UI.NewLabel(name), forceReadableName: true);
        Grid.AddChild(value, forceReadableName: true);
    }

    public void Add(string name, params Control[] items)
    {
        Grid.AddChild(UI.NewLabel(name), forceReadableName: true);
        Grid.AddChild(LayoutItems(), forceReadableName: true);

        Node LayoutItems()
        {
            var layout = UI.Layout(name);
            items.ForEach(x => layout.AddChild(x, forceReadableName: true));
            return layout;
        }
    }

    public void SetDisplayName(string name, string displayName)
        => UI.GetLabel(Grid, name).Text = displayName;

    public void ResetWidth()
        => Grid.ForEachChild<Control>(x => x.ResetWidth());
}
