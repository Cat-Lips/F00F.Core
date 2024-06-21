//#define DD

using System.Diagnostics;
using Godot;

namespace F00F
{
    public static class DebugDrawExtensions
    {
        private const string DD = "TOOLS"; // Change this to deactivate DebugDrawExtensions

        [Conditional(DD)]
        public static void DrawForce(this Node3D source, in Vector3 force)
        {
            if (force == Vector3.Zero) return;
#if DD
            DebugDraw3D.DrawArrow(source.GlobalPosition, source.ToGlobal(force), Color.Force, is_absolute_size: true);
#endif
        }

        [Conditional(DD)]
        public static void DrawVelocity(this Node3D source, in Vector3 velocity)
        {
            if (velocity == Vector3.Zero) return;
#if DD
            DebugDraw3D.DrawArrow(source.GlobalPosition, source.ToGlobal(velocity), Color.Velocity, is_absolute_size: true);
#endif
        }

        private static class Color
        {
            public static readonly Godot.Color Force = Colors.Blue;
            public static readonly Godot.Color Velocity = Colors.Purple;
        }
    }
}
