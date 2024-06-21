using Godot;

namespace F00F;

public static class VectorExtensions
{
    public static Vector2 XZ(this in Vector3 source)
        => new(source.X, source.Z);

    public static Vector3 FromXZ(this in Vector2 source, float y = 0)
        => new(source.X, 0, source.Y);

    public static bool IsZero(this in Vector3 source)
        => source.X is 0 && source.Y is 0 && source.Z is 0;

    public static Vector3 With(this in Vector3 source, float? x = null, float? y = null, float? z = null)
        => new(x ?? source.X, y ?? source.Y, z ?? source.Z);

    //public static Vector3 Flipped(this in Vector3 source, Vector3.Axis axis)
    //{
    //    var flipped = source;

    //    switch (axis)
    //    {
    //        case Vector3.Axis.X: flipped.X = -flipped.X; break;
    //        case Vector3.Axis.Y: flipped.Y = -flipped.Y; break;
    //        case Vector3.Axis.Z: flipped.Z = -flipped.Z; break;
    //    }

    //    return flipped;
    //}
}
