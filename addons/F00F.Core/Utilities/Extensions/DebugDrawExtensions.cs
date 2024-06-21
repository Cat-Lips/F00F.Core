using System.Diagnostics;
using Godot;

namespace F00F;

public static class DebugDraw
{
    #region Private

    private const int CutoffPrecision = 3;
    private const float DefaultSize = .1f;

    #endregion

    public static bool Enabled { get; set; }

    public enum Scope { Long, Short, Frame, Forever }

    #region Draw

    [Conditional("DEBUG_DRAW")]
    public static void DebugAabb(this Node3D self, in Aabb bb, Godot.Color? color = null, Scope scope = Scope.Frame)
    { if (Enabled) Draw(bb, color ?? Color.Aabb, scope); }

    [Conditional("DEBUG_DRAW")]
    public static void DebugPoint(this Node3D self, in Vector3 point, Godot.Color? color = null, float size = DefaultSize, Scope scope = Scope.Frame)
    { if (Enabled) Draw(point, color ?? Color.Point, size, scope); }

    #endregion

    #region DrawN

    [Conditional("DEBUG_DRAW")]
    public static void DebugAabbN(this Node3D self, in Aabb bb, Godot.Color? color = null, Scope scope = Scope.Frame)
    { if (Enabled) Draw(bb.TransformedBy(self.GlobalTransform), color ?? Color.Aabb, scope); }

    [Conditional("DEBUG_DRAW")]
    public static void DebugPointN(this Node3D self, in Vector3 point, Godot.Color? color = null, float size = DefaultSize, Scope scope = Scope.Frame)
    { if (Enabled) Draw(point.TransformedBy(self.GlobalTransform), color ?? Color.Point, size, scope); }

    #endregion

    #region Physics

    [Conditional("DEBUG_DRAW")]
    public static void DebugForce(this Node3D self, in Vector3 force, Godot.Color? color = null, Scope scope = Scope.Frame)
    {
        if (!Enabled) return;
        if (force.Rounded(CutoffPrecision) == Vector3.Zero) return;
        Draw(self.GlobalPosition, force, color ?? Color.Force, scope);
    }

    [Conditional("DEBUG_DRAW")]
    public static void DebugForce(this Node3D self, in Vector3 force, in Vector3 offset, Godot.Color? color = null, Scope scope = Scope.Frame)
    {
        if (!Enabled) return;
        if (force.Rounded(CutoffPrecision) == Vector3.Zero) return;
        Draw(self.GlobalPosition + offset, force, color ?? Color.Force, scope);
    }

    [Conditional("DEBUG_DRAW")]
    public static void DebugVelocity(this Node3D self, in Vector3 velocity, Godot.Color? color = null, Scope scope = Scope.Frame)
    {
        if (!Enabled) return;
        if (velocity.Rounded(CutoffPrecision) == Vector3.Zero) return;
        Draw(self.GlobalPosition, velocity, color ?? Color.Velocity, scope);
    }

    #endregion

    #region RigidBody3D

    [Conditional("DEBUG_DRAW")]
    public static void DebugCentralForce(this RigidBody3D self, in Vector3 force, Godot.Color? color = null, Scope scope = Scope.Frame)
        => self.DebugForce(force / self.Mass, color, scope);

    [Conditional("DEBUG_DRAW")]
    public static void DebugForce(this RigidBody3D self, in Vector3 force, in Vector3 offset, Godot.Color? color = null, Scope scope = Scope.Frame)
        => (self as Node3D).DebugForce(force / self.Mass, offset, color, scope);

    #endregion

    #region Private

#if DEBUG_DRAW
    private static void Draw(in Aabb bb, in Godot.Color color, Scope scope) => DebugDraw3D.DrawAabb(bb, color, Duration(scope));
    private static void Draw(in Vector3 point, in Godot.Color color, float size, Scope scope) => DebugDraw3D.DrawSphere(point, size, color, Duration(scope));
    private static void Draw(in Vector3 origin, in Vector3 length, in Godot.Color color, Scope scope) => DebugDraw3D.DrawArrow(origin, origin + length, color, DefaultSize, true, Duration(scope));

    static DebugDraw()
        => Log.Debug("DebugDraw: Active");
#else
    private static void Draw(in Aabb bb, in Godot.Color color, Scope scope) { }
    private static void Draw(in Vector3 point, in Godot.Color color, float size, Scope scope) { }
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

    private static float Duration(Scope scope) => scope switch
    {
        Scope.Long => 10,
        Scope.Short => 3,
        Scope.Frame => 0,
        Scope.Forever => float.MaxValue,
        _ => throw new System.NotImplementedException(),
    };

    #endregion
}
