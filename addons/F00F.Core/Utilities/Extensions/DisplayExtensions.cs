using System.Linq;
using Godot;
using static Godot.Control;

namespace F00F
{
    public static class DisplayExtensions
    {
        public static bool IsMouseOver(this Control source)
            => source.GetRect().HasPoint(source.GetLocalMousePosition());

        public static bool IsMouseOver(this Popup source)
            => source.Visible && new Rect2(default, source.Size).HasPoint(source.GetMousePosition());

        public static Control GuiGetHoveredControl(this Viewport source) // Coming in Godot 4.3
        {
            return source.RecurseChildren<Control>()
                .Where(x => x.MouseFilter != MouseFilterEnum.Ignore)
                .FirstOrDefault(x => x.IsMouseOver());
        }

        public static Rect2 GetDisplayRect(this Node source)
        {
            return Editor.IsEditor ? GetEditorRect() : GetRuntimeRect();

            Rect2 GetRuntimeRect()
                => source.GetMainWindowOrNull()?.GetVisibleRect() ?? GetEditorRect();

            static Rect2 GetEditorRect() => new(0, 0,
                (float)ProjectSettings.GetSettingWithOverride("display/window/size/viewport_width"),
                (float)ProjectSettings.GetSettingWithOverride("display/window/size/viewport_height"));
        }
    }
}
