using Godot;

namespace F00F;

using static Control;

public static partial class UI
{
    public static Separator Sep(bool v = false) => Sep("Sep1", v);
    public static Separator Sep(string name, bool v = false)
    {
        return v ? VSep() : HSep();

        VSeparator VSep() => new()
        {
            Name = name,
            SizeFlagsVertical = SizeFlags.ExpandFill,
        };

        HSeparator HSep() => new()
        {
            Name = name,
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
        };
    }

    public static Label Label(string name, string text = null, string hint = null, HorizontalAlignment align = HorizontalAlignment.Left, MouseFilterEnum? mouse = null) => new()
    {
        Name = $"{name}Label",
        Text = text ?? name.Capitalise(),
        TooltipText = hint,
        VerticalAlignment = VerticalAlignment.Center,
        HorizontalAlignment = align,
        MouseFilter = mouse ?? (hint is null ? MouseFilterEnum.Ignore : MouseFilterEnum.Pass),
    };
}
