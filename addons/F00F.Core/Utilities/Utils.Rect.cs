using Godot;

namespace F00F;

public readonly record struct Rect(float Top, float Left, float Right, float Bottom)
{
    public Vector2 TopLeft => new(Left, Top);
    public Vector2 TopRight => new(Right, Top);
    public Vector2 BottomLeft => new(Left, Bottom);
    public Vector2 BottomRight => new(Right, Bottom);

    public static Rect New(in Rect2 rect)
    {
        var end = rect.End;
        var pos = rect.Position;
        return new Rect(pos.Y, pos.X, end.X, end.Y);
    }

    public static Rect New(in Rect2 rect, in Vector2 inset)
    {
        var end = rect.End - inset;
        var pos = rect.Position + inset;
        return new Rect(pos.Y, pos.X, end.X, end.Y);
    }

    public static Rect2 Rect2(float top, float left, float right, float bottom)
        => new(left, top, right - left, bottom - top);
}
