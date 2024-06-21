using Godot;

namespace F00F;

public static class ThemeExtensions
{
    #region Label

    private static readonly StringName Font = "font";
    public static Label SetFont(this Label self, Font font)
        => self.SetThemeFont(Font, font);

    private static readonly StringName FontSize = "font_size";
    public static Label SetFontSize(this Label self, int? size)
        => self.SetThemeFontSize(FontSize, size);

    private static readonly StringName FontColor = "font_color";
    public static Label SetFontColor(this Label self, Color? color)
        => self.SetThemeColor(FontColor, color);

    private static readonly StringName ShadowOffsetX = "shadow_offset_x";
    private static readonly StringName ShadowOffsetY = "shadow_offset_y";
    private static readonly StringName ShadowColor = "font_shadow_color";
    private static readonly StringName ShadowSize = "shadow_outline_size";
    public static Label SetShadow(this Label self, bool? raised, int depth = 2, Color? color = null)
    {
        switch (raised)
        {
            case null:
                self.BeginBulkThemeOverride();
                self.RemoveThemeConstantOverride(ShadowOffsetX);
                self.RemoveThemeConstantOverride(ShadowOffsetY);
                self.RemoveThemeConstantOverride(ShadowSize);
                self.RemoveThemeColorOverride(ShadowColor);
                self.EndBulkThemeOverride();
                break;
            case true:
                self.BeginBulkThemeOverride();
                self.AddThemeConstantOverride(ShadowOffsetX, -depth);
                self.AddThemeConstantOverride(ShadowOffsetY, -depth);
                self.AddThemeConstantOverride(ShadowSize, self.GetThemeFontSize(FontSize) - depth);
                self.AddThemeColorOverride(ShadowColor, color ?? new Color(1, 1, 1, .5f));
                self.EndBulkThemeOverride();
                break;
            case false:
                self.BeginBulkThemeOverride();
                self.AddThemeConstantOverride(ShadowOffsetX, depth);
                self.AddThemeConstantOverride(ShadowOffsetY, depth);
                self.AddThemeConstantOverride(ShadowSize, self.GetThemeFontSize(FontSize) - depth);
                self.AddThemeColorOverride(ShadowColor, color ?? new Color(0, 0, 0, .6f));
                self.EndBulkThemeOverride();
                break;
        }

        return self;
    }

    private static readonly StringName OutlineSize = "outline_size";
    private static readonly StringName OutlineColor = "font_outline_color";
    public static Label SetOutline(this Label self, Color? color, int size = 2)
    {
        switch (color)
        {
            case null:
                self.BeginBulkThemeOverride();
                self.RemoveThemeColorOverride(OutlineColor);
                self.RemoveThemeConstantOverride(OutlineSize);
                self.EndBulkThemeOverride();
                break;
            default:
                self.BeginBulkThemeOverride();
                self.AddThemeColorOverride(OutlineColor, color.Value);
                self.AddThemeConstantOverride(OutlineSize, size);
                self.EndBulkThemeOverride();
                break;
        }

        return self;
    }

    public static Label SetTextColor(this Label self, Color? color) => self.SetFontColor(color);
    public static Label SetForeColor(this Label self, Color? color) => self.SetFontColor(color);
    //public static Label SetBackColor(this Label self, Color? color) { self.SelfModulate = color ?? Const.ModulateDflt; return self; }

    public static Label SetBackColor(this Label self, Color? color)
    {
        self.SetThemeStyleBox(StyleNormal, self.GetColorStyleOrNull(StyleNormal, color));
        return self;
    }

    #endregion

    #region Button

    private static readonly StringName FontHoverColor = "font_hover_color";
    private static readonly StringName FontFocusColor = "font_focus_color";
    private static readonly StringName FontNormalColor = "font_color";
    private static readonly StringName FontPressedColor = "font_pressed_color";
    public static Button SetFontColors(this Button self, Color? color)
    {
        self.BeginBulkThemeOverride();
        self.SetThemeColor(FontHoverColor, color);
        self.SetThemeColor(FontFocusColor, color);
        self.SetThemeColor(FontNormalColor, color);
        self.SetThemeColor(FontPressedColor, color);
        self.EndBulkThemeOverride();
        return self;
    }

    private static readonly StringName StyleHover = "hover";
    private static readonly StringName StyleFocus = "focus";
    private static readonly StringName StyleNormal = "normal";
    private static readonly StringName StylePressed = "pressed";
    public static Button SetStyleColors(this Button self, Color? color)
    {
        self.BeginBulkThemeOverride();
        self.SetThemeStyleBox(StyleHover, self.GetColorStyleOrNull(StyleHover, color));
        self.SetThemeStyleBox(StyleFocus, self.GetColorStyleOrNull(StyleFocus, color));
        self.SetThemeStyleBox(StyleNormal, self.GetColorStyleOrNull(StyleNormal, color));
        self.SetThemeStyleBox(StylePressed, self.GetColorStyleOrNull(StylePressed, color));
        self.EndBulkThemeOverride();
        return self;
    }

    public static Button SetTextColor(this Button self, Color? color) => self.SetFontColors(color);
    public static Button SetForeColor(this Button self, Color? color) => self.SetFontColors(color);
    public static Button SetBackColor(this Button self, Color? color) => self.SetStyleColors(color);

    #endregion

    #region Margin

    private static readonly StringName MarginTop = "margin_top";
    private static readonly StringName MarginLeft = "margin_left";
    private static readonly StringName MarginRight = "margin_right";
    private static readonly StringName MarginBottom = "margin_bottom";
    public static MarginContainer SetMargin(this MarginContainer self, int margin) => self.SetMargin(margin, margin, margin, margin);
    public static MarginContainer SetMargin(this MarginContainer self, in Vector2I size) => self.SetMargin(size.Y, size.X, size.X, size.Y);
    public static MarginContainer SetMargin(this MarginContainer self, int? top = null, int? left = null, int? right = null, int? bottom = null)
    {
        self.BeginBulkThemeOverride();
        self.SetThemeConst(MarginTop, top);
        self.SetThemeConst(MarginLeft, left);
        self.SetThemeConst(MarginRight, right);
        self.SetThemeConst(MarginBottom, bottom);
        self.EndBulkThemeOverride();
        return self;
    }

    #endregion

    #region LineEdit

    private static readonly StringName UneditableFontColor = "font_uneditable_color";
    public static LineEdit SetUneditableFontColor(this LineEdit self, Color? color = null)
        => self.SetThemeColor(UneditableFontColor, color ?? self.GetThemeColor(FontColor));

    #endregion

    #region Helpers

    private static T SetThemeFont<T>(this T self, StringName key, Font font = null) where T : Control
    {
        if (font is null) self.RemoveThemeFontOverride(key);
        else self.AddThemeFontOverride(key, font);
        return self;
    }

    private static T SetThemeFontSize<T>(this T self, StringName key, int? size = null) where T : Control
    {
        if (size is null) self.RemoveThemeFontSizeOverride(key);
        else self.AddThemeFontSizeOverride(key, size.Value);
        return self;
    }

    private static T SetThemeColor<T>(this T self, StringName key, Color? color = null) where T : Control
    {
        if (color is null) self.RemoveThemeColorOverride(key);
        else self.AddThemeColorOverride(key, color.Value);
        return self;
    }

    private static T SetThemeConst<T>(this T self, StringName key, int? value = null) where T : Control
    {
        if (value is null) self.RemoveThemeConstantOverride(key);
        else self.AddThemeConstantOverride(key, value.Value);
        return self;
    }

    private static T SetThemeIcon<T>(this T self, StringName key, Texture2D icon = null) where T : Control
    {
        if (icon is null) self.RemoveThemeIconOverride(key);
        else self.AddThemeIconOverride(key, icon);
        return self;
    }

    private static T SetThemeStyleBox<T>(this T self, StringName key, StyleBox style = null) where T : Control
    {
        if (style is null) self.RemoveThemeStyleboxOverride(key);
        else self.AddThemeStyleboxOverride(key, style);
        return self;
    }

    private static StyleBoxFlat GetColorStyleOrNull(this Control self, StringName key, Color? color = null)
    {
        if (color is null) return null;
        var sb = self.GetThemeStylebox(key) as StyleBoxFlat;
        sb = sb is null ? new() : sb.Copy();
        sb.BgColor = color.Value;
        return sb;
    }

    #endregion
}
