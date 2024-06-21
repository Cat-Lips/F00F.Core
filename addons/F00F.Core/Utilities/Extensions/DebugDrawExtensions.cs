using System.Diagnostics;
using Godot;

namespace F00F;

public static class DebugDraw
{
    public static bool Enabled { get; set; }

    public enum Scope
    {
        Long,
        Short,
        Frame,
        Forever
    }

    #region RigidBody3D

    [Conditional("DEBUG_DRAW_3D")]
    public static void _DrawCentralForce(this RigidBody3D source, in Vector3 force, Godot.Color? color = null, Scope scope = Scope.Frame)
        => source.DrawForce(force / source.Mass, color, scope);

    [Conditional("DEBUG_DRAW_3D")]
    public static void _DrawForce(this RigidBody3D source, in Vector3 force, in Vector3 position, Godot.Color? color = null, Scope scope = Scope.Frame)
        => source.DrawForce(force / source.Mass, position, color, scope);

    #endregion

    [Conditional("DEBUG_DRAW_3D")]
    public static void DrawBB(this Node3D source, in Aabb bb, Godot.Color? color = null, Scope scope = Scope.Frame)
    {
        if (!Enabled) return;
        Draw(bb.TransformedBy(source.GlobalTransform), color ?? Color.Aabb, scope);
    }

    [Conditional("DEBUG_DRAW_3D")]
    public static void DrawPoint(this Node3D source, in Vector3 position, float size = DefaultSize, Godot.Color? color = null, Scope scope = Scope.Frame)
    {
        if (!Enabled) return;
        Draw(position, size, color ?? Color.Point, scope);
    }

    [Conditional("DEBUG_DRAW_3D")]
    public static void DrawForce(this Node3D source, in Vector3 force, Godot.Color? color = null, Scope scope = Scope.Frame)
    {
        if (!Enabled) return;
        if (force.Rounded(CutoffPrecision) == Vector3.Zero) return;
        Draw(source.GlobalPosition, force, color ?? Color.Force, scope);
    }

    [Conditional("DEBUG_DRAW_3D")]
    public static void DrawForce(this Node3D source, in Vector3 force, in Vector3 offset, Godot.Color? color = null, Scope scope = Scope.Frame)
    {
        if (!Enabled) return;
        if (force.Rounded(CutoffPrecision) == Vector3.Zero) return;
        Draw(source.GlobalPosition + offset, force, color ?? Color.Force, scope);
    }

    [Conditional("DEBUG_DRAW_3D")]
    public static void DrawVelocity(this Node3D source, in Vector3 velocity, Godot.Color? color = null, Scope scope = Scope.Frame)
    {
        if (!Enabled) return;
        if (velocity.Rounded(CutoffPrecision) == Vector3.Zero) return;
        Draw(source.GlobalPosition, velocity, color ?? Color.Velocity, scope);
    }

#if DEBUG_DRAW_3D
    private static void Draw(in Aabb bb, in Godot.Color color, Scope scope) => DebugDraw3D.DrawAabb(bb, color, Duration(scope));
    private static void Draw(in Vector3 point, float size, in Godot.Color color, Scope scope) => DebugDraw3D.DrawSphere(point, size, color, Duration(scope));
    private static void Draw(in Vector3 origin, in Vector3 length, in Godot.Color color, Scope scope) => DebugDraw3D.DrawArrow(origin, origin + length, color, DefaultSize, true, Duration(scope));

    static DebugDraw()
        => Log.Debug("DebugDraw: Active");
#else
    private static void Draw(in Aabb bb, in Godot.Color color, Scope scope) { }
    private static void Draw(in Vector3 point, float size, in Godot.Color color, Scope scope) { }
    private static void Draw(in Vector3 origin, in Vector3 length, in Godot.Color color, Scope scope) { }
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

    private static float Duration(Scope scope) => scope switch
    {
        Scope.Long => 10,
        Scope.Short => 3,
        Scope.Frame => 0,
        Scope.Forever => float.MaxValue,
        _ => throw new System.NotImplementedException(),
    };
}
