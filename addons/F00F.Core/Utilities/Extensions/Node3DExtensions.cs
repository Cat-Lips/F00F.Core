using Godot;

namespace F00F
{
    public static class Node3DExtensions
    {
        public static Vector3 Forward(this Node3D source) => source.GlobalTransform.Forward();
        public static Vector3 Back(this Node3D source) => source.GlobalTransform.Back();
        public static Vector3 Left(this Node3D source) => source.GlobalTransform.Left();
        public static Vector3 Right(this Node3D source) => source.GlobalTransform.Right();
        public static Vector3 Up(this Node3D source) => source.GlobalTransform.Up();
        public static Vector3 Down(this Node3D source) => source.GlobalTransform.Down();

        public static Vector3 Forward(this Transform3D source) => -source.Basis.Z;
        public static Vector3 Back(this Transform3D source) => source.Basis.Z;
        public static Vector3 Left(this Transform3D source) => -source.Basis.X;
        public static Vector3 Right(this Transform3D source) => source.Basis.X;
        public static Vector3 Up(this Transform3D source) => -source.Basis.Y;
        public static Vector3 Down(this Transform3D source) => source.Basis.Y;

        public static bool IsMovingForward(this Node3D source, in Vector3 velocity, float epsilon = 0) => source.GetRelative(velocity).Z < -epsilon;
        public static bool IsMovingBack(this Node3D source, in Vector3 velocity, float epsilon = 0) => source.GetRelative(velocity).Z > epsilon;
        public static bool IsMovingLeft(this Node3D source, in Vector3 velocity, float epsilon = 0) => source.GetRelative(velocity).X < -epsilon;
        public static bool IsMovingRight(this Node3D source, in Vector3 velocity, float epsilon = 0) => source.GetRelative(velocity).X > epsilon;
        public static bool IsMovingDown(this Node3D source, in Vector3 velocity, float epsilon = 0) => source.GetRelative(velocity).Y < -epsilon;
        public static bool IsMovingUp(this Node3D source, in Vector3 velocity, float epsilon = 0) => source.GetRelative(velocity).Y > epsilon;

        public static Vector3 GetRelative(this Node3D source, in Vector3 velocity) => source.GlobalBasis * velocity;

        public static Transform3D GlobalTransform(this Node3D node)
        {
            return node.TopLevel || !node.HasParent(out Node3D parent)
                ? node.Transform : parent.GlobalTransform() * node.Transform;
        }

        public static void GlobalTransform(this Node3D node, in Transform3D transform)
        {
            node.Transform = node.TopLevel || !node.HasParent(out Node3D parent)
                ? transform : parent.GlobalTransform().AffineInverse() * transform;
        }

        private static bool HasParent<T>(this Node source, out T parent) where T : class
            => (parent = source.GetParentOrNull<T>()) is not null;

        public static Node3D Top(this Node3D source) => source.Top<Node3D>();
        public static T Top<T>(this Node3D source) where T : Node3D
        {
            var top = source as T;
            if (source.TopLevel) return top;
            var parent = source.GetParentOrNull<Node3D>();
            return parent is null ? top : parent.Top<T>() ?? top;
        }
    }
}
