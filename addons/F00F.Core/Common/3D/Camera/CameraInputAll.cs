using Godot;

namespace F00F;

[Tool, GlobalClass]
public partial class CameraInputAll : CameraInput
{
    protected internal sealed override Vector3 GetMovement() => new(
        MyInput.GetAxis(MyInput.Left, MyInput.Right),
        MyInput.GetAxis(MyInput.Down, MyInput.Up),
        MyInput.GetAxis(MyInput.Forward, MyInput.Back));

    protected internal sealed override bool ZoomIn()
        => MyInput.IsActionJustPressed(MyInput.ZoomIn);

    protected internal sealed override bool ZoomOut()
        => MyInput.IsActionJustPressed(MyInput.ZoomOut);

    protected internal sealed override bool Fast1()
        => MyInput.IsActionPressed(MyInput.Fast1);

    protected internal sealed override bool Fast2()
        => MyInput.IsActionPressed(MyInput.Fast2);

    protected internal sealed override bool Fast3()
        => MyInput.IsActionPressed(MyInput.Fast3);

    protected internal sealed override bool SelectJustPressed()
        => MyInput.IsActionJustPressed(MyInput.Select);

    protected internal sealed override bool ToggleSelectMode()
        => MyInput.IsActionJustPressed(MyInput.ToggleSelectMode);

    protected internal sealed override bool ToggleOrbitMode()
        => MyInput.IsActionJustPressed(MyInput.ToggleOrbitMode);

    protected internal sealed override bool ResetCameraPosition()
        => MyInput.IsActionJustPressed(MyInput.Reset);

    protected internal sealed override bool MouseRotate(Camera3D self, InputEvent e)
        => MyInput.MouseRotate(self, e, self.Config.Sensitivity, self.Config.PitchLimit);

    protected internal sealed override bool MouseOrbit(Camera3D self, InputEvent e, in Vector3 target)
        => MyInput.MouseOrbit(self, e, target, self.Config.Sensitivity, self.Config.PitchLimit);
}
