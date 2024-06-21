using System.Collections.Generic;
using Godot;
using ControlPair = (Godot.Control Label, Godot.Control EditControl);

namespace F00F;

[Tool]
public partial class DataView : Root
{
    public GridContainer Grid => field ??= GetNode<GridContainer>("%Grid");

    public void Add(IEnumerable<ControlPair> content)
        => Grid.Init(content);

    public void AddSep() => Grid.AddRowSeparator();
    public void AddRowSeparator() => Grid.AddRowSeparator();

    public void Add(Control value) => Add(value.Name, value);
    public void Add(string name, Control value)
    {
        Grid.AddChild(UI.NewLabel(name), forceReadableName: true);
        Grid.AddChild(value, forceReadableName: true);
    }

    public void SetDisplayName(string name, string displayName)
        => Grid.GetNode<Label>($"{name}Label").Text = displayName;
}
