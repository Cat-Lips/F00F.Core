using System;
using Godot;

namespace F00F;

public static class MathExtensions_Round
{
    private const MidpointRounding DefaultRoundType = MidpointRounding.AwayFromZero;

    public static float Round(this float source, MidpointRounding type = DefaultRoundType)
        => float.Round(source, type);

    public static float Round(this float source, int digits, MidpointRounding type = DefaultRoundType)
        => float.Round(source, digits, type);

    public static int RoundInt(this float source, MidpointRounding type = DefaultRoundType)
        => (int)source.Round(type);

    public static Vector2 Round(this in Vector2 source, MidpointRounding type = DefaultRoundType)
        => new(source.X.Round(type), source.Y.Round(type));

    public static Vector2 Round(this in Vector2 source, int digits, MidpointRounding type = DefaultRoundType)
        => new(source.X.Round(digits, type), source.Y.Round(digits, type));

    public static Vector3 Round(this in Vector3 source, MidpointRounding type = DefaultRoundType)
        => new(source.X.Round(type), source.Y.Round(type), source.Z.Round(type));

    public static Vector3 Round(this in Vector3 source, int digits, MidpointRounding type = DefaultRoundType)
        => new(source.X.Round(digits, type), source.Y.Round(digits, type), source.Z.Round(digits, type));

    public static Vector3 RoundXZ(this in Vector3 source, float y = 0, MidpointRounding type = DefaultRoundType)
        => new(source.X.Round(type), y, source.Z.Round(type));

    public static Vector2 TrueRound(this in Vector2 source, MidpointRounding type = DefaultRoundType) => Round(source, type);
    public static Vector3 TrueRound(this in Vector3 source, MidpointRounding type = DefaultRoundType) => Round(source, type);
}
