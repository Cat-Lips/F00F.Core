using Godot;

namespace F00F;

[Tool, GlobalClass]
public partial class PlayerInput : CustomResource
{
    protected internal virtual Vector2 Movement()
    {
        return MyInput.GetVector(
            MyInput.StrafeLeft, MyInput.StrafeRight,
            MyInput.MoveForward, MyInput.MoveBack);
    }

    protected internal virtual bool Run()
        => MyInput.IsActionPressed(MyInput.Run);

    protected internal virtual bool Jump()
        => MyInput.IsActionJustPressed(MyInput.Jump);

    protected internal virtual bool Shoot()
        => MyInput.IsActionPressed(MyInput.Shoot);

    protected internal virtual bool Interact()
        => MyInput.IsActionJustPressed(MyInput.Interact);
}
