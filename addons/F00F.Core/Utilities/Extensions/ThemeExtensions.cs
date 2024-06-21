using Godot;

namespace F00F
{
    public static class ThemeExtensions
    {
        #region Font

        private static readonly StringName FontColor = "font_color";
        private static readonly StringName UneditableFontColor = "font_uneditable_color";

        public static T SetFontColor<T>(this T source, in Color? color) where T : Control
            => source.SetThemeColor(FontColor, color);

        public static LineEdit OverrideUneditableFontColor(this LineEdit source, in Color? color = null)
            => source.SetThemeColor(UneditableFontColor, color ?? source.GetThemeColor(FontColor));

        private static T SetThemeColor<T>(this T source, StringName theme, in Color? color) where T : Control
        {
            if (color is null) Reset();
            else Set(color.Value);
            return source;

            void Set(in Color color)
                => source.AddThemeColorOverride(theme, color);

            void Reset()
                => source.RemoveThemeColorOverride(theme);
        }

        #endregion

        #region Margin

        private static readonly StringName MarginTop = "margin_top";
        private static readonly StringName MarginLeft = "margin_left";
        private static readonly StringName MarginRight = "margin_right";
        private static readonly StringName MarginBottom = "margin_bottom";

        public static MarginContainer SetMargin(this MarginContainer source, int margin) => source.SetMargin(margin, margin, margin, margin);
        public static MarginContainer SetMargin(this MarginContainer source, int top, int left, int right, int bottom)
        {
            source.BeginBulkThemeOverride();

            Set(MarginTop, top);
            Set(MarginLeft, left);
            Set(MarginRight, right);
            Set(MarginBottom, bottom);

            source.EndBulkThemeOverride();
            return source;

            void Set(StringName key, int value)
            {
                if (value is 0) source.RemoveThemeConstantOverride(key);
                else source.AddThemeConstantOverride(key, value);
            }
        }

        #endregion
    }
}
