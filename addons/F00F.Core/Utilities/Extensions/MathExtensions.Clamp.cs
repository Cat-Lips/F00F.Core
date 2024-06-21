using Godot;

namespace F00F;

public static class MathExtensions_Clamp
{
    public static int ClampMin(this int value, int min) => Mathf.Max(value, min);
    public static int ClampMax(this int value, int max) => Mathf.Min(value, max);
    public static int Clamp(this int value, int min, int max) => Mathf.Clamp(value, min, max);
}
