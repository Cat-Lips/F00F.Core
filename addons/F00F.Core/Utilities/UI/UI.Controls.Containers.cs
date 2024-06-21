using System.Collections.Generic;
using Godot;

namespace F00F;

using static BoxContainer;
using static Control;

public static partial class UI
{
    public static HBoxContainer Layout(string name) => new()
    {
        Name = $"{name}Layout",
        Alignment = AlignmentMode.End,
        SizeFlagsVertical = SizeFlags.ShrinkCenter,
        SizeFlagsHorizontal = SizeFlags.ExpandFill,
    };

    public static HBoxContainer Layout(string name, params IEnumerable<Control> kids)
    {
        var layout = Layout(name);
        kids.ForEach(x => layout.AddChild(x, forceReadableName: true));
        return layout;
    }
}
