﻿using Godot;

namespace F00F;

public static class MathExtensions_Clamp
{
    public static int ClampMin(this int value, int min) => Mathf.Max(value, min);
    public static int ClampMax(this int value, int max) => Mathf.Min(value, max);
    public static int Clamp(this int value, int min, int max) => Mathf.Clamp(value, min, max);

    public static float ClampMin(this float value, float min) => Mathf.Max(value, min);
    public static float ClampMax(this float value, float max) => Mathf.Min(value, max);
    public static float Clamp(this float value, float min, float max) => Mathf.Clamp(value, min, max);
}
