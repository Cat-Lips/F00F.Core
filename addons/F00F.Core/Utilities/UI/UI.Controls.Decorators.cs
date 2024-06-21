using Godot;

namespace F00F;

using static Control;
using static TextureRect;

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

    public static LineEdit SunkLabel(string name, string text, Color? textColor = null, Color? backColor = null, string hint = null) => SunkLabel(name, text, hint, textColor, backColor);
    public static LineEdit SunkLabel(string name, string text = null, string hint = null, Color? fontColor = null, Color? selfColor = null) => new LineEdit()
    {
        Name = name,
        Text = text ?? name.Capitalise(),
        Editable = false,
        TooltipText = hint,
        SelfModulate = selfColor ?? Const.ModulateDflt,
        ExpandToTextLength = true,
        ContextMenuEnabled = false,
    }.SetUneditableFontColor(fontColor);

    public static TextureRect TextureRect(string name, string texture, string hint = null)
        => TextureRect(name, F00F.Utils.Load<Texture2D>(texture), hint);

    public static TextureRect TextureRect(string name, Texture2D texture = null, string hint = null) => new()
    {
        Name = name,
        Texture = texture,
        ExpandMode = ExpandModeEnum.FitWidthProportional,
        StretchMode = StretchModeEnum.KeepAspectCentered,
        TooltipText = hint,
    };
}
