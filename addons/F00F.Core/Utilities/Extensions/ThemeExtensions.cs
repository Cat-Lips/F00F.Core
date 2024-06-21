using Godot;

namespace F00F;

public static class ThemeExtensions
{
    #region Font (old)

    //private static readonly StringName FontColor = "font_color";
    private static readonly StringName UneditableFontColor = "font_uneditable_color";

    public static T ResetFontColor<T>(this T self) where T : Control => self.SetFontColor(null);
    public static T SetFontColor<T>(this T self, Color? color) where T : Control
        => self.SetThemeColor(FontColor, color);

    public static LineEdit OverrideUneditableFontColor(this LineEdit self, Color? color = null)
        => self.SetThemeColor(UneditableFontColor, color ?? self.GetThemeColor(FontColor));

    private static T SetThemeColor<T>(this T self, StringName theme, Color? color = null) where T : Control
    {
        if (color is null) Reset();
        else Set(color.Value);
        return self;

        void Set(in Color color)
            => self.AddThemeColorOverride(theme, color);

        void Reset()
            => self.RemoveThemeColorOverride(theme);
    }

    #endregion

    #region Margin

    private static readonly StringName MarginTop = "margin_top";
    private static readonly StringName MarginLeft = "margin_left";
    private static readonly StringName MarginRight = "margin_right";
    private static readonly StringName MarginBottom = "margin_bottom";

    public static MarginContainer SetMargin(this MarginContainer self, int margin) => self.SetMargin(margin, margin, margin, margin);
    public static MarginContainer SetMargin(this MarginContainer self, in Vector2I size) => self.SetMargin(size.Y, size.X, size.X, size.Y);
    public static MarginContainer SetMargin(this MarginContainer self, int top, int left, int right, int bottom)
    {
        self.BeginBulkThemeOverride();

        Set(MarginTop, top);
        Set(MarginLeft, left);
        Set(MarginRight, right);
        Set(MarginBottom, bottom);

        self.EndBulkThemeOverride();
        return self;

        void Set(StringName key, int value)
        {
            if (value is 0) self.RemoveThemeConstantOverride(key);
            else self.AddThemeConstantOverride(key, value);
        }
    }

    #endregion

    #region Label

    private static readonly StringName Font = "font";
    public static Label SetFont(this Label self, Font font = null)
    {
        if (font is null) self.RemoveThemeFontOverride(Font);
        else self.AddThemeFontOverride(Font, font);
        return self;
    }

    private static readonly StringName FontSize = "font_size";
    public static Label SetFontSize(this Label self, int fontSize = 0)
    {
        if (fontSize is <= 0) self.RemoveThemeFontSizeOverride(FontSize);
        else self.AddThemeFontSizeOverride(FontSize, fontSize);
        return self;
    }

    private static readonly StringName FontColor = "font_color";
    public static Label SetFontColor(this Label self, Color? fontColor = null)
    {
        if (fontColor is null) self.RemoveThemeColorOverride(FontColor);
        else self.AddThemeColorOverride(FontColor, fontColor.Value);
        return self;
    }

    private static readonly StringName ShadowOffsetX = "shadow_offset_x";
    private static readonly StringName ShadowOffsetY = "shadow_offset_y";
    private static readonly StringName FontShadowColor = "font_shadow_color";
    public static Label SetFontShadow(this Label self, bool? raised = null, int depth = 2)
    {
        switch (raised)
        {
            case null:
                self.RemoveThemeConstantOverride(ShadowOffsetX);
                self.RemoveThemeConstantOverride(ShadowOffsetY);
                self.RemoveThemeColorOverride(FontShadowColor);
                break;
            case true:
                self.AddThemeConstantOverride(ShadowOffsetX, -depth);
                self.AddThemeConstantOverride(ShadowOffsetY, -depth);
                self.AddThemeColorOverride(FontShadowColor, new Color(1, 1, 1, .5f));
                break;
            case false:
                self.AddThemeConstantOverride(ShadowOffsetX, depth);
                self.AddThemeConstantOverride(ShadowOffsetY, depth);
                self.AddThemeColorOverride(FontShadowColor, new Color(0, 0, 0, .6f));
                break;
        }

        return self;
    }

    #endregion
}
