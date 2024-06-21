using System.Linq;
using Godot;

namespace F00F
{
    public static class DisplayExtensions
    {
        public static Rect2 GetDisplayRect(this Node source)
        {
            return Engine.IsEditorHint() ? GetEditorRect() : GetRuntimeRect();

            Rect2 GetRuntimeRect()
                => source.GetMainWindowOrNull()?.GetVisibleRect() ?? GetEditorRect();

            static Rect2 GetEditorRect() => new(0, 0,
                (float)ProjectSettings.GetSettingWithOverride("display/window/size/viewport_width"),
                (float)ProjectSettings.GetSettingWithOverride("display/window/size/viewport_height"));
        }

        public static bool MouseOver(this Control source)
            => source.GetGlobalRect().HasPoint(source.GetGlobalMousePosition());

        public static Control GuiGetHoveredControl(this Viewport source)
            => source.RecurseChildren<Control>().FirstOrDefault(x => x.MouseOver());
    }
}
