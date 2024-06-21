using System.Diagnostics;
using Godot;

namespace F00F;

public static class DebugDraw
{
    public static bool Enabled { get; set; }

    #region RigidBody3D

    [Conditional("DEBUG_DRAW_3D")]
    public static void _DrawCentralForce(this Node3D source, in Vector3 force, Godot.Color? color = null)
        => source.DrawForce(force, color);

    [Conditional("DEBUG_DRAW_3D")]
    public static void _DrawForce(this Node3D source, in Vector3 force, in Vector3 position, Godot.Color? color = null)
        => source.DrawForce(force, position, color);

    #endregion

    [Conditional("DEBUG_DRAW_3D")]
    public static void DrawAabb(this Node3D source, in Aabb bb, bool xform = true, Godot.Color? color = null)
    {
        if (!Enabled) return;
        Draw(xform ? bb.TransformedBy(source.GlobalTransform) : bb, color ?? Color.Aabb);
    }

    [Conditional("DEBUG_DRAW_3D")]
    public static void DrawPoint(this Node3D source, in Vector3 position, float size = DefaultSize, Godot.Color? color = null)
    {
        if (!Enabled) return;
        Draw(position, size, color ?? Color.Point);
    }

    [Conditional("DEBUG_DRAW_3D")]
    public static void DrawForce(this Node3D source, in Vector3 force, Godot.Color? color = null)
    {
        if (!Enabled) return;
        if (force.Rounded(CutoffPrecision) == Vector3.Zero) return;
        Draw(source.GlobalPosition, force, color ?? Color.Force);
    }

    [Conditional("DEBUG_DRAW_3D")]
    public static void DrawForce(this Node3D source, in Vector3 force, in Vector3 offset, Godot.Color? color = null)
    {
        if (!Enabled) return;
        if (force.Rounded(CutoffPrecision) == Vector3.Zero) return;
        Draw(source.GlobalPosition + offset, force, color ?? Color.Force);
    }

    [Conditional("DEBUG_DRAW_3D")]
    public static void DrawVelocity(this Node3D source, in Vector3 velocity, Godot.Color? color = null)
    {
        if (!Enabled) return;
        if (velocity.Rounded(CutoffPrecision) == Vector3.Zero) return;
        Draw(source.GlobalPosition, velocity, color ?? Color.Velocity);
    }

#if DEBUG_DRAW_3D
    private static void Draw(in Aabb bb, in Godot.Color color) => DebugDraw3D.DrawAabb(bb, color);
    private static void Draw(in Vector3 point, float size, in Godot.Color color) => DebugDraw3D.DrawSphere(point, size, color);
    private static void Draw(in Vector3 origin, in Vector3 length, in Godot.Color color) => DebugDraw3D.DrawArrow(origin, origin + length, color, DefaultSize, true);

    static DebugDraw()
        => Log.Debug("DebugDraw: Active");
#else
    private static void Draw(in Aabb bb, in Godot.Color color) { }
    private static void Draw(in Vector3 point, float size, in Godot.Color color) { }
    private static void Draw(in Vector3 origin, in Vector3 length, in Godot.Color color) { }
#endif

    private static class Color
    {
        public static readonly Godot.Color Aabb = Colors.Gray.With(a: .5f);

        public static readonly Godot.Color Point = Colors.Yellow.With(a: .5f);
        public static readonly Godot.Color Rotation = Colors.Yellow.With(a: .5f);

        public static readonly Godot.Color Force = Colors.Green.With(a: .5f);
        public static readonly Godot.Color Velocity = Colors.Blue.With(a: .5f);
    }

    public const int CutoffPrecision = 3;
    public const float DefaultSize = .1f;
}
