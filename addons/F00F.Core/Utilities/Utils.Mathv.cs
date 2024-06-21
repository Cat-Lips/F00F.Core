using System.Runtime.CompilerServices;
using Godot;

namespace F00F;

public static class Mathv
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Max(in Vector2 a, in Vector2 b)
        => new(Mathf.Max(a.X, b.X), Mathf.Max(a.Y, b.Y));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2I Max(in Vector2I a, in Vector2I b)
        => new(Mathf.Max(a.X, b.X), Mathf.Max(a.Y, b.Y));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 Max(in Vector3 a, in Vector3 b)
        => new(Mathf.Max(a.X, b.X), Mathf.Max(a.Y, b.Y), Mathf.Max(a.Z, b.Z));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3I Max(in Vector3I a, in Vector3I b)
        => new(Mathf.Max(a.X, b.X), Mathf.Max(a.Y, b.Y), Mathf.Max(a.Z, b.Z));
}
