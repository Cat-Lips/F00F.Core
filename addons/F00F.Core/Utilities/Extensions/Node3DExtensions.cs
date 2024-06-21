using System.Runtime.CompilerServices;
using Godot;

namespace F00F;

public static class Node3DExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 Fwd(this in Basis basis) => -basis.Z;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 Back(this in Basis basis) => basis.Z;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 Left(this in Basis basis) => -basis.X;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 Right(this in Basis basis) => basis.X;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 Down(this in Basis basis) => -basis.Y;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 Up(this in Basis basis) => basis.Y;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 Side(this in Basis basis) => basis.Right();

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 Fwd(this in Transform3D gform) => gform.Basis.Fwd();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 Back(this in Transform3D gform) => gform.Basis.Back();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 Left(this in Transform3D gform) => gform.Basis.Left();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 Right(this in Transform3D gform) => gform.Basis.Right();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 Down(this in Transform3D gform) => gform.Basis.Down();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 Up(this in Transform3D gform) => gform.Basis.Up();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 Pos(this in Transform3D gform) => gform.Origin;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 Side(this in Transform3D gform) => gform.Basis.Side();

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 Fwd(this Node3D node) => node.GlobalBasis.Fwd();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 Back(this Node3D node) => node.GlobalBasis.Back();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 Left(this Node3D node) => node.GlobalBasis.Left();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 Right(this Node3D node) => node.GlobalBasis.Right();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 Down(this Node3D node) => node.GlobalBasis.Down();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 Up(this Node3D node) => node.GlobalBasis.Up();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 Pos(this Node3D node) => node.GlobalPosition;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 Side(this Node3D node) => node.GlobalBasis.Side();

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Fwd(this in Basis basis, in Vector3 vec) => vec.Dot(basis.Fwd());
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Back(this in Basis basis, in Vector3 vec) => vec.Dot(basis.Back());
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Left(this in Basis basis, in Vector3 vec) => vec.Dot(basis.Left());
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Right(this in Basis basis, in Vector3 vec) => vec.Dot(basis.Right());
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Down(this in Basis basis, in Vector3 vec) => vec.Dot(basis.Down());
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Up(this in Basis basis, in Vector3 vec) => vec.Dot(basis.Up());

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Fwd(this in Transform3D gform, in Vector3 vec) => gform.Basis.Fwd(vec);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Back(this in Transform3D gform, in Vector3 vec) => gform.Basis.Back(vec);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Left(this in Transform3D gform, in Vector3 vec) => gform.Basis.Left(vec);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Right(this in Transform3D gform, in Vector3 vec) => gform.Basis.Right(vec);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Down(this in Transform3D gform, in Vector3 vec) => gform.Basis.Down(vec);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Up(this in Transform3D gform, in Vector3 vec) => gform.Basis.Up(vec);

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Fwd(this Node3D node, in Vector3 vec) => node.GlobalBasis.Fwd(vec);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Back(this Node3D node, in Vector3 vec) => node.GlobalBasis.Back(vec);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Left(this Node3D node, in Vector3 vec) => node.GlobalBasis.Left(vec);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Right(this Node3D node, in Vector3 vec) => node.GlobalBasis.Right(vec);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Down(this Node3D node, in Vector3 vec) => node.GlobalBasis.Down(vec);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Up(this Node3D node, in Vector3 vec) => node.GlobalBasis.Up(vec);

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsFwd(this in Basis basis, in Vector3 vec) => basis.Fwd(vec) > Const.Epsilon;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsBack(this in Basis basis, in Vector3 vec) => basis.Back(vec) > Const.Epsilon;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsLeft(this in Basis basis, in Vector3 vec) => basis.Left(vec) > Const.Epsilon;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsRight(this in Basis basis, in Vector3 vec) => basis.Right(vec) > Const.Epsilon;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsDown(this in Basis basis, in Vector3 vec) => basis.Down(vec) > Const.Epsilon;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsUp(this in Basis basis, in Vector3 vec) => basis.Up(vec) > Const.Epsilon;

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsFwd(this in Transform3D gform, in Vector3 vec) => gform.Basis.IsFwd(vec);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsBack(this in Transform3D gform, in Vector3 vec) => gform.Basis.IsBack(vec);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsLeft(this in Transform3D gform, in Vector3 vec) => gform.Basis.IsLeft(vec);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsRight(this in Transform3D gform, in Vector3 vec) => gform.Basis.IsRight(vec);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsDown(this in Transform3D gform, in Vector3 vec) => gform.Basis.IsDown(vec);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsUp(this in Transform3D gform, in Vector3 vec) => gform.Basis.IsUp(vec);

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsFwd(this Node3D node, in Vector3 vec) => node.GlobalBasis.IsFwd(vec);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsBack(this Node3D node, in Vector3 vec) => node.GlobalBasis.IsBack(vec);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsLeft(this Node3D node, in Vector3 vec) => node.GlobalBasis.IsLeft(vec);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsRight(this Node3D node, in Vector3 vec) => node.GlobalBasis.IsRight(vec);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsDown(this Node3D node, in Vector3 vec) => node.GlobalBasis.IsDown(vec);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsUp(this Node3D node, in Vector3 vec) => node.GlobalBasis.IsUp(vec);

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsMovingFwd(this in Basis basis, in Vector3 vel) => basis.IsFwd(vel);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsMovingBack(this in Basis basis, in Vector3 vel) => basis.IsBack(vel);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsMovingLeft(this in Basis basis, in Vector3 vel) => basis.IsLeft(vel);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsMovingRight(this in Basis basis, in Vector3 vel) => basis.IsRight(vel);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsMovingDown(this in Basis basis, in Vector3 vel) => basis.IsDown(vel);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsMovingUp(this in Basis basis, in Vector3 vel) => basis.IsUp(vel);

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsMovingFwd(this in Transform3D gform, in Vector3 vel) => gform.Basis.IsMovingFwd(vel);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsMovingBack(this in Transform3D gform, in Vector3 vel) => gform.Basis.IsMovingBack(vel);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsMovingLeft(this in Transform3D gform, in Vector3 vel) => gform.Basis.IsMovingLeft(vel);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsMovingRight(this in Transform3D gform, in Vector3 vel) => gform.Basis.IsMovingRight(vel);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsMovingDown(this in Transform3D gform, in Vector3 vel) => gform.Basis.IsMovingDown(vel);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsMovingUp(this in Transform3D gform, in Vector3 vel) => gform.Basis.IsMovingUp(vel);

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsMovingFwd(this Node3D node, in Vector3 vel) => node.GlobalBasis.IsMovingFwd(vel);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsMovingBack(this Node3D node, in Vector3 vel) => node.GlobalBasis.IsMovingBack(vel);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsMovingLeft(this Node3D node, in Vector3 vel) => node.GlobalBasis.IsMovingLeft(vel);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsMovingRight(this Node3D node, in Vector3 vel) => node.GlobalBasis.IsMovingRight(vel);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsMovingDown(this Node3D node, in Vector3 vel) => node.GlobalBasis.IsMovingDown(vel);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsMovingUp(this Node3D node, in Vector3 vel) => node.GlobalBasis.IsMovingUp(vel);

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsMoving(this in Vector3 vel, in Vector3 dir) => vel.IsMoving(dir);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsMovingIn(this in Vector3 vel, in Vector3 dir) => vel.Dot(dir) >= 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsMovingTowards(this in Vector3 pos, in Vector3 target, in Vector3 vel)
        => vel.Dot(target - pos) > Const.Epsilon;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsMovingTowards(this Node3D source, in Vector3 target, in Vector3 vel)
        => source.GlobalPosition.IsMovingTowards(target, vel);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsMovingAwayFrom(this in Vector3 pos, in Vector3 target, in Vector3 vel)
        => vel.Dot(target - pos) < -Const.Epsilon;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsMovingAwayFrom(this Node3D source, in Vector3 target, in Vector3 vel)
        => source.GlobalPosition.IsMovingAwayFrom(target, vel);

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsFwd(this in Vector3 pos) => pos.Z < 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsBack(this in Vector3 pos) => pos.Z > 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsLeft(this in Vector3 pos) => pos.X < 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsRight(this in Vector3 pos) => pos.X > 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsDown(this in Vector3 pos) => pos.Y < 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsUp(this in Vector3 pos) => pos.Y > 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsFront(this in Vector3 pos) => pos.IsFwd();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsBehind(this in Vector3 pos) => pos.IsBack();

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsFwd(this Node3D node) => node.Position.IsFwd();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsBack(this Node3D node) => node.Position.IsBack();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsLeft(this Node3D node) => node.Position.IsLeft();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsRight(this Node3D node) => node.Position.IsRight();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsDown(this Node3D node) => node.Position.IsDown();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsUp(this Node3D node) => node.Position.IsUp();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsFront(this Node3D node) => node.IsFwd();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsBehind(this Node3D node) => node.IsBack();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWithinAngle(this in Vector3 from, in Vector3 to, float angle)
        => from.IsWithinCos(to, Mathf.Cos(angle));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWithinAngle(this in Vector3 from, in Vector3 to, float minAngle, float maxAngle)
        => from.IsWithinCos(to, Mathf.Cos(minAngle), Mathf.Cos(maxAngle));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWithinCos(this in Vector3 from, in Vector3 to, float cos)
        => from.Dot(to) >= cos;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWithinCos(this in Vector3 from, in Vector3 to, float minCos, float maxCos)
    {
        var alignment = from.Dot(to);
        return alignment <= minCos && alignment >= maxCos;
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
