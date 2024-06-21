using Godot;

namespace F00F;

public static class MathExtensions_Negate
{
    public static int Negate(this int source, bool negate = true) => negate ? -source : source;
    public static float Negate(this float source, bool negate = true) => negate ? -source : source;

    public static int Min(this int value, int min) => Mathf.Max(value, min);
    public static int Max(this int value, int max) => Mathf.Min(value, max);
    public static int Clamp(this int value, int min, int max) => Mathf.Clamp(value, min, max);
}
