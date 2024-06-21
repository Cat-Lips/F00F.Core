using Godot;

namespace F00F;

public static class MathExtensions_Clamp
{
    public static int ClampMin(this int value, int min) => Mathf.Max(value, min);
    public static int ClampMax(this int value, int max) => Mathf.Min(value, max);
    public static int Clamp(this int value, int min = 0, int max = 1) => Mathf.Clamp(value, min, max);

    public static float ClampMin(this float value, float min) => Mathf.Max(value, min);
    public static float ClampMax(this float value, float max) => Mathf.Min(value, max);
    public static float Clamp(this float value, float min = 0, float max = 1) => Mathf.Clamp(value, min, max);

    public static int Min(this int a, int b) => Mathf.Min(a, b);
    public static int Max(this int a, int b) => Mathf.Max(a, b);

    public static float Min(this float a, float b) => Mathf.Min(a, b);
    public static float Max(this float a, float b) => Mathf.Max(a, b);

    public static int Abs(this int s) => Mathf.Abs(s);
    public static int Sign(this int s) => Mathf.Sign(s);
    public static float Abs(this float s) => Mathf.Abs(s);
    public static float Sign(this float s) => Mathf.Sign(s);

    public static Vector2 ClampMin(this in Vector2 value, float min) => value.Max(min);
    public static Vector2 ClampMax(this in Vector2 value, float max) => value.Min(max);
    public static Vector2 Clamp(this Vector2 value, float min = 0, float max = 1) => value.Clamp(min, max);

    public static Vector2I ClampMin(this in Vector2I value, int min) => value.Max(min);
    public static Vector2I ClampMax(this in Vector2I value, int max) => value.Min(max);
    public static Vector2I Clamp(this Vector2I value, int min = 0, int max = 1) => value.Clamp(min, max);

    public static Vector3 ClampMin(this in Vector3 value, float min) => value.Max(min);
    public static Vector3 ClampMax(this in Vector3 value, float max) => value.Min(max);
    public static Vector3 Clamp(this Vector3 value, float min = 0, float max = 1) => value.Clamp(min, max);

    public static Vector3I ClampMin(this in Vector3I value, int min) => value.Max(min);
    public static Vector3I ClampMax(this in Vector3I value, int max) => value.Min(max);
    public static Vector3I Clamp(this Vector3I value, int min = 0, int max = 1) => value.Clamp(min, max);

    public static float Min(this in Vector2 v) => Mathf.Min(v.X, v.Y);
    public static float Max(this in Vector2 v) => Mathf.Max(v.X, v.Y);
    public static float Min(this in Vector2I v) => Mathf.Min(v.X, v.Y);
    public static float Max(this in Vector2I v) => Mathf.Max(v.X, v.Y);

    public static float Min(this in Vector3 v) => Mathf.Min(Mathf.Min(v.X, v.Y), v.Z);
    public static float Max(this in Vector3 v) => Mathf.Max(Mathf.Max(v.X, v.Y), v.Z);
    public static float Min(this in Vector3I v) => Mathf.Min(Mathf.Min(v.X, v.Y), v.Z);
    public static float Max(this in Vector3I v) => Mathf.Max(Mathf.Max(v.X, v.Y), v.Z);
}
