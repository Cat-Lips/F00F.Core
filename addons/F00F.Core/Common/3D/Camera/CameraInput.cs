using Godot;

namespace F00F;

[Tool, GlobalClass]
public partial class CameraInput : CustomResource
{
    protected internal virtual Vector3 GetMovement() => new(
        MyInput.GetAxis(MyInput.Left, MyInput.Right),
        MyInput.GetAxis(MyInput.Down, MyInput.Up),
        MyInput.GetAxis(MyInput.Forward, MyInput.Back));

    protected internal virtual bool ZoomIn()
        => MyInput.IsActionJustPressed(MyInput.ZoomIn);

    protected internal virtual bool ZoomOut()
        => MyInput.IsActionJustPressed(MyInput.ZoomOut);

    protected internal virtual bool Fast1()
        => MyInput.IsActionPressed(MyInput.Fast1);

    protected internal virtual bool Fast2()
        => MyInput.IsActionPressed(MyInput.Fast2);

    protected internal virtual bool Fast3()
        => MyInput.IsActionPressed(MyInput.Fast3);

    protected internal virtual bool SelectJustPressed()
        => MyInput.IsActionJustPressed(MyInput.Select);

    protected internal virtual bool ToggleSelectMode()
        => MyInput.IsActionJustPressed(MyInput.ToggleSelectMode);

    protected internal virtual bool ToggleOrbitMode()
        => MyInput.IsActionJustPressed(MyInput.ToggleOrbitMode);

    protected internal virtual bool ResetCameraPosition()
        => MyInput.IsActionJustPressed(MyInput.Reset);
}
