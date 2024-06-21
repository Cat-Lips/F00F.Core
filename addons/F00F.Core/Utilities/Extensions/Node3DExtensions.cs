using Godot;

namespace F00F;

public static class Node3DExtensions
{
    public static Vector3 Fwd(this Node3D source) => source.GlobalTransform.Fwd();
    public static Vector3 Back(this Node3D source) => source.GlobalTransform.Back();
    public static Vector3 Left(this Node3D source) => source.GlobalTransform.Left();
    public static Vector3 Right(this Node3D source) => source.GlobalTransform.Right();
    public static Vector3 Down(this Node3D source) => source.GlobalTransform.Down();
    public static Vector3 Up(this Node3D source) => source.GlobalTransform.Up();

    public static Vector3 Fwd(this Transform3D source) => -source.Basis.Z;
    public static Vector3 Back(this Transform3D source) => source.Basis.Z;
    public static Vector3 Left(this Transform3D source) => -source.Basis.X;
    public static Vector3 Right(this Transform3D source) => source.Basis.X;
    public static Vector3 Down(this Transform3D source) => -source.Basis.Y;
    public static Vector3 Up(this Transform3D source) => source.Basis.Y;

    public static bool IsFwd(this Node3D source) => source.Position.Z < 0;
    public static bool IsBack(this Node3D source) => source.Position.Z > 0;
    public static bool IsLeft(this Node3D source) => source.Position.X < 0;
    public static bool IsRight(this Node3D source) => source.Position.X > 0;
    public static bool IsDown(this Node3D source) => source.Position.Y < 0;
    public static bool IsUp(this Node3D source) => source.Position.Y > 0;
    public static bool IsFront(this Node3D source) => source.IsFwd();

    public static bool IsMovingFwd(this Node3D source, in Vector3 velocity) => velocity.Dot(source.Fwd()) > Const.Epsilon;
    public static bool IsMovingBack(this Node3D source, in Vector3 velocity) => velocity.Dot(source.Back()) > Const.Epsilon;
    public static bool IsMovingLeft(this Node3D source, in Vector3 velocity) => velocity.Dot(source.Left()) > Const.Epsilon;
    public static bool IsMovingRight(this Node3D source, in Vector3 velocity) => velocity.Dot(source.Right()) > Const.Epsilon;
    public static bool IsMovingDown(this Node3D source, in Vector3 velocity) => velocity.Dot(source.Down()) > Const.Epsilon;
    public static bool IsMovingUp(this Node3D source, in Vector3 velocity) => velocity.Dot(source.Up()) > Const.Epsilon;

    public static Node3D Top(this Node3D source) => source.Top<Node3D>();
    public static T Top<T>(this Node3D source) where T : Node3D
    {
        var top = source as T;
        if (source.TopLevel) return top;
        var parent = source.GetParentOrNull<Node3D>();
        return parent is null ? top : parent.Top<T>() ?? top;
    }

    public static Transform3D GlobalTransform(this Node3D source)
    {
        return source.TopLevel || !source.HasParent(out Node3D parent)
            ? source.Transform : source.Transform.TransformedBy(parent.GlobalTransform());
    }

    public static void GlobalTransform(this Node3D source, in Transform3D transform)
    {
        source.Transform = source.TopLevel || !source.HasParent(out Node3D parent)
            ? transform : transform.TransformedBy(parent.GlobalTransform().AffineInverse());
    }
}
