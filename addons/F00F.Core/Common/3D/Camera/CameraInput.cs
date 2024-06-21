using Godot;

namespace F00F;

[Tool, GlobalClass]
public abstract partial class CameraInput : CustomResource
{
    protected internal virtual Vector3 GetMovement() => Vector3.Zero;
    protected internal virtual bool ZoomIn() => false;
    protected internal virtual bool ZoomOut() => false;
    protected internal virtual bool Fast1() => false;
    protected internal virtual bool Fast2() => false;
    protected internal virtual bool Fast3() => false;
    protected internal virtual bool SelectJustPressed() => false;
    protected internal virtual bool ToggleSelectMode() => false;
    protected internal virtual bool ToggleOrbitMode() => false;
    protected internal virtual bool ResetCameraPosition() => false;
}
