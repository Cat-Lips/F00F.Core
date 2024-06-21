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
    public static Vector3 Side(this Node3D source) => source.GlobalTransform.Side();
    public static Vector3 Pos(this Node3D source) => source.GlobalTransform.Pos();

    public static Vector3 Fwd(this in Transform3D source) => -source.Basis.Z;
    public static Vector3 Back(this in Transform3D source) => source.Basis.Z;
    public static Vector3 Left(this in Transform3D source) => -source.Basis.X;
    public static Vector3 Right(this in Transform3D source) => source.Basis.X;
    public static Vector3 Down(this in Transform3D source) => -source.Basis.Y;
    public static Vector3 Up(this in Transform3D source) => source.Basis.Y;
    public static Vector3 Side(this in Transform3D source) => source.Right();
    public static Vector3 Pos(this in Transform3D source) => source.Origin;

    public static bool IsFwd(this Node3D source) => source.Position.Z < 0;
    public static bool IsBack(this Node3D source) => source.Position.Z > 0;
    public static bool IsLeft(this Node3D source) => source.Position.X < 0;
    public static bool IsRight(this Node3D source) => source.Position.X > 0;
    public static bool IsDown(this Node3D source) => source.Position.Y < 0;
    public static bool IsUp(this Node3D source) => source.Position.Y > 0;
    public static bool IsFront(this Node3D source) => source.IsFwd();
    public static bool IsBehind(this Node3D source) => source.IsBack();

    public static bool IsFwd(this in Vector3 source) => source.Z < 0;
    public static bool IsBack(this in Vector3 source) => source.Z > 0;
    public static bool IsLeft(this in Vector3 source) => source.X < 0;
    public static bool IsRight(this in Vector3 source) => source.X > 0;
    public static bool IsDown(this in Vector3 source) => source.Y < 0;
    public static bool IsUp(this in Vector3 source) => source.Y > 0;
    public static bool IsFront(this in Vector3 source) => source.IsFwd();
    public static bool IsBehind(this in Vector3 source) => source.IsBack();

    public static bool IsMovingFwd(this Node3D source, in Vector3 velocity) => velocity.Dot(source.Fwd()) > Const.Epsilon;
    public static bool IsMovingBack(this Node3D source, in Vector3 velocity) => velocity.Dot(source.Back()) > Const.Epsilon;
    public static bool IsMovingLeft(this Node3D source, in Vector3 velocity) => velocity.Dot(source.Left()) > Const.Epsilon;
    public static bool IsMovingRight(this Node3D source, in Vector3 velocity) => velocity.Dot(source.Right()) > Const.Epsilon;
    public static bool IsMovingDown(this Node3D source, in Vector3 velocity) => velocity.Dot(source.Down()) > Const.Epsilon;
    public static bool IsMovingUp(this Node3D source, in Vector3 velocity) => velocity.Dot(source.Up()) > Const.Epsilon;

    public static bool IsMovingTowards(this Node3D source, in Vector3 target, in Vector3 velocity)
        => velocity.Dot(target - source.GlobalPosition) > Const.Epsilon;

    public static bool IsMovingAwayFrom(this Node3D source, in Vector3 target, in Vector3 velocity)
        => velocity.Dot(target - source.GlobalPosition) < -Const.Epsilon;

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
