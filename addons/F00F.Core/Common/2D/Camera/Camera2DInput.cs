using Godot;

namespace F00F;

[Tool, GlobalClass]
public partial class Camera2DInput : CustomResource
{
    protected internal virtual Vector2 Movement()
        => MyInput.GetVector(MyInput.Left, MyInput.Right, MyInput.Up, MyInput.Down);

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

    protected internal virtual bool Select()
        => MyInput.IsActionJustPressed(MyInput.Select);

    protected internal virtual bool ToggleSelectMode()
        => MyInput.IsActionJustPressed(MyInput.ToggleSelectMode);

    protected internal virtual bool ResetCameraPosition()
        => MyInput.IsActionJustPressed(MyInput.Reset);
}
