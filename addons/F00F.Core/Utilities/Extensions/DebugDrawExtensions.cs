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
    public static void DebugAabb(this Node3D self, in Aabb bb, Color? color = null, Scope scope = Scope.Frame)
    { if (Enabled) Draw(bb, color ?? Clr.Aabb, scope); }

    [Conditional("DEBUG_DRAW")]
    public static void DebugPoint(this Node3D self, in Vector3 point, Color? color = null, float size = DefaultSize, Scope scope = Scope.Frame)
    { if (Enabled) Draw(point, color ?? Clr.Point, size, scope); }

    #endregion

    #region DrawN

    [Conditional("DEBUG_DRAW")]
    public static void DebugAabbN(this Node3D self, in Aabb bb, Color? color = null, Scope scope = Scope.Frame)
    { if (Enabled) Draw(bb.TransformedBy(self.GlobalTransform), color ?? Clr.Aabb, scope); }

    [Conditional("DEBUG_DRAW")]
    public static void DebugPointN(this Node3D self, in Vector3 point, Color? color = null, float size = DefaultSize, Scope scope = Scope.Frame)
    { if (Enabled) Draw(point.TransformedBy(self.GlobalTransform), color ?? Clr.Point, size, scope); }

    #endregion

    #region Physics

    [Conditional("DEBUG_DRAW")]
    public static void DebugForce(this Node3D self, in Vector3 force, Color? color = null, Scope scope = Scope.Frame)
    {
        if (!Enabled) return;
        if (force.Rounded(CutoffPrecision) == Vector3.Zero) return;
        Draw(self.GlobalPosition, force, color ?? Clr.Force, scope);
    }

    [Conditional("DEBUG_DRAW")]
    public static void DebugForce(this Node3D self, in Vector3 force, in Vector3 offset, Color? color = null, Scope scope = Scope.Frame)
    {
        if (!Enabled) return;
        if (force.Rounded(CutoffPrecision) == Vector3.Zero) return;
        Draw(self.GlobalPosition + offset, force, color ?? Clr.Force, scope);
    }

    [Conditional("DEBUG_DRAW")]
    public static void DebugNormal(this Node3D self, in Vector3 normal, Color? color = null, Scope scope = Scope.Frame)
    {
        if (!Enabled) return;
        if (normal.Rounded(CutoffPrecision) == Vector3.Zero) return;
        Draw(self.GlobalPosition, normal, color ?? Clr.Normal, scope);
    }

    [Conditional("DEBUG_DRAW")]
    public static void DebugVelocity(this Node3D self, in Vector3 velocity, Color? color = null, Scope scope = Scope.Frame)
    {
        if (!Enabled) return;
        if (velocity.Rounded(CutoffPrecision) == Vector3.Zero) return;
        Draw(self.GlobalPosition, velocity, color ?? Clr.Velocity, scope);
    }

    #endregion

    #region RigidBody3D

    [Conditional("DEBUG_DRAW")]
    public static void DebugCentralForce(this RigidBody3D self, in Vector3 force, Color? color = null, Scope scope = Scope.Frame)
        => self.DebugForce(force / self.Mass, color, scope);

    [Conditional("DEBUG_DRAW")]
    public static void DebugForce(this RigidBody3D self, in Vector3 force, in Vector3 offset, Color? color = null, Scope scope = Scope.Frame)
        => (self as Node3D).DebugForce(force / self.Mass, offset, color, scope);

    #endregion

    #region Private

    private static class Clr
    {
        public static readonly Color Aabb = Colors.Gray.With(a: .5f);

        public static readonly Color Point = Colors.Yellow.With(a: .5f);
        public static readonly Color Normal = Colors.Yellow.With(a: .5f);

        public static readonly Color Force = Colors.Green.With(a: .5f);
        public static readonly Color Velocity = Colors.Blue.With(a: .5f);
    }

#if DEBUG_DRAW
    private static void Draw(in Aabb bb, in Color color, Scope scope) => DebugDraw3D.DrawAabb(bb, color, Duration(scope));
    private static void Draw(in Vector3 point, in Color color, float size, Scope scope) => DebugDraw3D.DrawSphere(point, size, color, Duration(scope));
    private static void Draw(in Vector3 origin, in Vector3 length, in Color color, Scope scope) => DebugDraw3D.DrawArrow(origin, origin + length, color, DefaultSize, true, Duration(scope));

    static DebugDraw()
    {
        Enabled = true;
        Log.Debug("DebugDraw: Active");
    }

    private static float Duration(Scope scope) => scope switch
    {
        Scope.Long => 10,
        Scope.Short => 3,
        Scope.Frame => 0,
        Scope.Forever => float.MaxValue,
        _ => throw new System.NotImplementedException(),
    };
#else
    private static void Draw(in Aabb bb, in Color color, Scope scope) { }
    private static void Draw(in Vector3 point, in Color color, float size, Scope scope) { }
    private static void Draw(in Vector3 origin, in Vector3 length, in Color color, Scope scope) { }
#endif

    #endregion
}
