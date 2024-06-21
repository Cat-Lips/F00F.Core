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

    protected internal virtual bool Reset()
        => MyInput.IsActionJustPressed(MyInput.Reset);

    protected internal virtual bool Select()
        => MyInput.IsActionJustPressed(MyInput.Select);

    protected internal virtual bool SelectMode()
        => MyInput.IsActionJustPressed(MyInput.SelectMode);

    protected internal virtual bool MouseRotate(Camera3D self, InputEvent e)
        => MyInput.MouseRotate(self, e, self.Config.Sensitivity, self.Config.PitchLimit);

    protected internal virtual bool MouseOrbit(Camera3D self, InputEvent e, in Node3D target, ref Vector3 lookAt)
        => MyInput.MouseOrbit(self, e, target.GlobalTransform, ref lookAt, true, self.Config.Sensitivity, self.Config.OrbitPitchLimit);
}
