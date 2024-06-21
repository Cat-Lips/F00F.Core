using Godot;

namespace F00F;

[Tool, GlobalClass]
public partial class FreeCam : CustomResource
{
    protected internal virtual Vector3 GetDirection() => new(
        MyInput.GetAxis(MyInput.Left, MyInput.Right),
        MyInput.GetAxis(MyInput.Down, MyInput.Up),
        MyInput.GetAxis(MyInput.Forward, MyInput.Back));

    protected internal virtual bool SpeedUp()
        => MyInput.IsActionJustPressed(MyInput.SpeedUp);

    protected internal virtual bool SlowDown()
        => MyInput.IsActionJustPressed(MyInput.SlowDown);
}
