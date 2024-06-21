using Godot;

namespace F00F;

[Tool, GlobalClass]
public partial class CameraInputAll : CameraInput
{
    protected internal override Vector3 GetMovement() => new(
        MyInput.GetAxis(MyInput.Left, MyInput.Right),
        MyInput.GetAxis(MyInput.Down, MyInput.Up),
        MyInput.GetAxis(MyInput.Forward, MyInput.Back));

    protected internal override bool ZoomIn()
        => MyInput.IsActionJustPressed(MyInput.ZoomIn);

    protected internal override bool ZoomOut()
        => MyInput.IsActionJustPressed(MyInput.ZoomOut);

    protected internal override bool Fast1()
        => MyInput.IsActionPressed(MyInput.Fast1);

    protected internal override bool Fast2()
        => MyInput.IsActionPressed(MyInput.Fast2);

    protected internal override bool Fast3()
        => MyInput.IsActionPressed(MyInput.Fast3);

    protected internal override bool SelectJustPressed()
        => MyInput.IsActionJustPressed(MyInput.Select);

    protected internal override bool ToggleSelectMode()
        => MyInput.IsActionJustPressed(MyInput.ToggleSelectMode);

    protected internal override bool ToggleOrbitMode()
        => MyInput.IsActionJustPressed(MyInput.ToggleOrbitMode);

    protected internal override bool ResetCameraPosition()
        => MyInput.IsActionJustPressed(MyInput.Reset);
}
