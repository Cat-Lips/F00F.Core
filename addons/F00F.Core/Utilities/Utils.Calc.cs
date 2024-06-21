using System;
using Godot;

namespace F00F;

public static class Calc
{
    public static float BetterLerp(float from, float to, float weight)
        => Mathf.Lerp(to, from, Mathf.Pow(2, -weight));

    public static Vector2 BetterLerp(in Vector2 from, in Vector2 to, float weight)
        => new(BetterLerp(from.X, to.X, weight), BetterLerp(from.Y, to.Y, weight));

    internal static Vector2 BetterLerp(Vector2 zero, Vector2 velocity, object value)
        => throw new NotImplementedException();
}
