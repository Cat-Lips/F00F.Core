using System.Linq;
using Godot;

namespace F00F;

using Camera = Godot.Camera2D;

public static class CameraExtensions
{
    public static void SetLimits(this Camera self, in Rect2I bb, bool smooth = true)
        => self.SetLimits(bb.Position, bb.End, smooth);

    public static void SetLimits(this Camera self, in Vector2I size, bool smooth = true)
        => self.SetLimits(default, size, smooth);

    public static void SetLimits(this Camera self, in Vector2I pos, in Vector2I end, bool smooth = true)
    {
        self.LimitTop = pos.Y;
        self.LimitLeft = pos.X;
        self.LimitRight = end.X;
        self.LimitBottom = end.Y;

        self.LimitEnabled = true;
        self.LimitSmoothed = smooth;
        self.EditorDrawLimits = true;
        self.PositionSmoothingEnabled |= smooth;
        //self.RotationSmoothingEnabled |= smooth;
    }

    public static void SetLimits(this Camera self, Node bg, bool smooth = true)
        => self.SetLimits((Rect2I)bg
            .RecurseChildren<Sprite2D>(self: true)
            .Select(x => x.GetRect()).Aggregate((a, b) => a.Merge(b)), smooth);
}
