using System.Diagnostics;
using Godot;

namespace F00F
{
    public static class DebugDrawExtensions
    {
        private static class Color
        {
            public static readonly Godot.Color Aabb = Colors.Gray;
            public static readonly Godot.Color Point = Colors.DarkGray;
            public static readonly Godot.Color Force = Colors.Blue;
            public static readonly Godot.Color Velocity = Colors.Purple;
        }

        [Conditional("DEBUG_DRAW_3D")]
        public static void DrawAabb(this Node3D source, in Aabb bb, bool xform = true)
            => Draw(xform ? bb.TransformedBy(source.GlobalTransform) : bb, Color.Aabb);

        [Conditional("DEBUG_DRAW_3D")]
        public static void DrawForce(this Node3D source, in Vector3 force)
        {
            if (force.Round(3) == Vector3.Zero) return;
            Draw(source.GlobalPosition - force, force, Color.Force);
        }

        [Conditional("DEBUG_DRAW_3D")]
        public static void DrawVelocity(this Node3D source, in Vector3 velocity)
        {
            if (velocity.Round(3) == Vector3.Zero) return;
            Draw(source.GlobalPosition, velocity, Color.Velocity);
        }

#if DEBUG_DRAW_3D
        private static void Draw(in Aabb bb, in Godot.Color color) => DebugDraw3D.DrawAabb(bb, color);
        private static void Draw(in Vector3 origin, in Vector3 length, in Godot.Color color) => DebugDraw3D.DrawArrow(origin, origin + length, color, is_absolute_size: true);
#else
        private static void Draw(in Aabb bb, in Godot.Color color) { }
        private static void Draw(in Vector3 origin, in Vector3 length, in Godot.Color color) { }
#endif
    }
}
