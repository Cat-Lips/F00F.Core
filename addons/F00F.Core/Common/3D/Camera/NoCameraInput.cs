using Godot;

namespace F00F;

[Tool, GlobalClass]
public partial class NoCameraInput : CameraInput
{
    protected internal sealed override Vector3 GetMovement() => Vector3.Zero;
    protected internal sealed override bool ZoomIn() => false;
    protected internal sealed override bool ZoomOut() => false;
    protected internal sealed override bool Fast1() => false;
    protected internal sealed override bool Fast2() => false;
    protected internal sealed override bool Fast3() => false;
    protected internal sealed override bool Reset() => false;
    protected internal sealed override bool Select() => false;
    protected internal sealed override bool SelectMode() => false;
    protected internal sealed override bool MouseRotate(Camera3D self, InputEvent e) => false;
    protected internal sealed override bool MouseOrbit(Camera3D self, InputEvent e, in Node3D target, ref Vector3 lookAt) => false;
}
