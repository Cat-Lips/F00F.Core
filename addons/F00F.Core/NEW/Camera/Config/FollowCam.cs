using Godot;

namespace F00F;

[Tool, GlobalClass]
public partial class FollowCam : CustomResource
{
    //protected internal virtual bool ZoomIn()
    //    => MyInput.IsActionJustPressed(MyInput.ZoomIn);

    //protected internal virtual bool ZoomOut()
    //    => MyInput.IsActionJustPressed(MyInput.ZoomOut);

    protected internal virtual bool OrbitLock()
        => MyInput.IsActionJustPressed(MyInput.OrbitLock);

    protected internal virtual bool LookBehind()
        => MyInput.IsActionPressed(MyInput.LookBehind);
}
