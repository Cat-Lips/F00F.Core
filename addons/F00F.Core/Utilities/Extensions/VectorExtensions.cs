using System;
using Godot;

namespace F00F;

public static class VectorExtensions
{
    public static Vector2 V2(this in Vector3 source) => source.XZ();
    public static Vector2 Vec2(this in Vector3 source) => source.XZ();
    public static Vector2 XZ(this in Vector3 source) => new(source.X, source.Z);

    public static Vector3 V3(this in Vector2 source, float y = 0) => source.FromXZ(y);
    public static Vector3 Vec3(this in Vector2 source, float y = 0) => source.FromXZ(y);
    public static Vector3 FromXZ(this in Vector2 source, float y = 0) => new(source.X, y, source.Y);

    public static bool IsZero(this in Vector3 source)
        => source.X is 0 && source.Y is 0 && source.Z is 0;

    public static Vector3 With(this in Vector3 source, float? x = null, float? y = null, float? z = null)
        => new(x ?? source.X, y ?? source.Y, z ?? source.Z);

    public static Vector3 Add(this in Vector3 source, float x = 0, float y = 0, float z = 0)
        => new(source.X + x, source.Y + y, source.Z + z);

    public static float[] ToArray(this in Vector3 source)
        => [source.X, source.Y, source.Z];

    public static bool IsUniform(this in Vector3 source, float tolerance = Const.TinyFloat)
    {
        return Mathf.IsEqualApprox(source.X, source.Y, tolerance) &&
               Mathf.IsEqualApprox(source.X, source.Z, tolerance);
    }

    public static float GetUniformValue(this in Vector3 source, float tolerance = Const.TinyFloat)
    {
        return source.IsUniform(tolerance)
            ? source.ToArray().GetMostRoundValue()
            : throw new Exception($"Expected uniform XYZ in Vec3: {source}");
    }

    public static Vector3 Unify(this in Vector3 source, float tolerance = Const.TinyFloat)
        => Vector3.One * source.GetUniformValue(tolerance);

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
