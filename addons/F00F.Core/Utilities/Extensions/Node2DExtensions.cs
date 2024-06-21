using Godot;

namespace F00F;

public static class Node2DExtensions
{
    public static Vector2 Fwd(this Node2D source) => source.GlobalTransform.Fwd();
    public static Vector2 Back(this Node2D source) => source.GlobalTransform.Back();
    public static Vector2 Left(this Node2D source) => source.GlobalTransform.Left();
    public static Vector2 Right(this Node2D source) => source.GlobalTransform.Right();
    public static Vector2 Down(this Node2D source) => source.GlobalTransform.Down();
    public static Vector2 Up(this Node2D source) => source.GlobalTransform.Up();

    public static Vector2 Fwd(this in Transform2D source) => source.Right();
    public static Vector2 Back(this in Transform2D source) => source.Left();
    public static Vector2 Left(this in Transform2D source) => -source.X;
    public static Vector2 Right(this in Transform2D source) => source.X;
    public static Vector2 Down(this in Transform2D source) => source.Y;
    public static Vector2 Up(this in Transform2D source) => -source.Y;

    public static bool IsFwd(this Node2D source) => source.IsRight();
    public static bool IsBack(this Node2D source) => source.IsLeft();
    public static bool IsLeft(this Node2D source) => source.Position.X < 0;
    public static bool IsRight(this Node2D source) => source.Position.X > 0;
    public static bool IsDown(this Node2D source) => source.Position.Y > 0;
    public static bool IsUp(this Node2D source) => source.Position.Y < 0;
    public static bool IsFront(this Node2D source) => source.IsFwd();

    public static bool IsMovingFwd(this Node2D source, in Vector2 velocity) => velocity.Dot(source.Fwd()) > Const.Epsilon;
    public static bool IsMovingBack(this Node2D source, in Vector2 velocity) => velocity.Dot(source.Back()) > Const.Epsilon;
    public static bool IsMovingLeft(this Node2D source, in Vector2 velocity) => velocity.Dot(source.Left()) > Const.Epsilon;
    public static bool IsMovingRight(this Node2D source, in Vector2 velocity) => velocity.Dot(source.Right()) > Const.Epsilon;
    public static bool IsMovingDown(this Node2D source, in Vector2 velocity) => velocity.Dot(source.Down()) > Const.Epsilon;
    public static bool IsMovingUp(this Node2D source, in Vector2 velocity) => velocity.Dot(source.Up()) > Const.Epsilon;
}
