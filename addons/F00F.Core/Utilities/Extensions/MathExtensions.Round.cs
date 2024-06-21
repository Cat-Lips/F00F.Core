using System;
using Godot;

namespace F00F;

public static class MathExtensions_Round
{
    private const MidpointRounding DefaultRoundType = MidpointRounding.AwayFromZero;

    public static float Rounded(this float source, MidpointRounding type = DefaultRoundType)
        => float.Round(source, type);

    public static float Rounded(this float source, int digits, MidpointRounding type = DefaultRoundType)
        => float.Round(source, digits, type);

    public static int RoundInt(this float source, MidpointRounding type = DefaultRoundType)
        => (int)source.Rounded(type);

    public static double Rounded(this double source, MidpointRounding type = DefaultRoundType)
        => double.Round(source, type);

    public static double Rounded(this double source, int digits, MidpointRounding type = DefaultRoundType)
        => double.Round(source, digits, type);

    public static int RoundInt(this double source, MidpointRounding type = DefaultRoundType)
        => (int)source.Rounded(type);

    public static Vector2 Rounded(this in Vector2 source, MidpointRounding type = DefaultRoundType)
        => new(source.X.Rounded(type), source.Y.Rounded(type));

    public static Vector2 Rounded(this in Vector2 source, int digits, MidpointRounding type = DefaultRoundType)
        => new(source.X.Rounded(digits, type), source.Y.Rounded(digits, type));

    public static Vector3 Rounded(this in Vector3 source, MidpointRounding type = DefaultRoundType)
        => new(source.X.Rounded(type), source.Y.Rounded(type), source.Z.Rounded(type));

    public static Vector3 Rounded(this in Vector3 source, int digits, MidpointRounding type = DefaultRoundType)
        => new(source.X.Rounded(digits, type), source.Y.Rounded(digits, type), source.Z.Rounded(digits, type));

    public static Vector2 RoundXZ(this in Vector3 source, MidpointRounding type = DefaultRoundType)
        => new(source.X.Rounded(type), source.Z.Rounded(type));

    public static Vector2 RoundXZ(this in Vector3 source, int digits, MidpointRounding type = DefaultRoundType)
        => new(source.X.Rounded(digits, type), source.Z.Rounded(digits, type));

    public static Vector3 RoundXZ(this in Vector3 source, float y, MidpointRounding type = DefaultRoundType)
        => new(source.X.Rounded(type), y, source.Z.Rounded(type));

    public static Vector3 RoundXZ(this in Vector3 source, float y, int digits, MidpointRounding type = DefaultRoundType)
        => new(source.X.Rounded(digits, type), y, source.Z.Rounded(digits, type));
}
