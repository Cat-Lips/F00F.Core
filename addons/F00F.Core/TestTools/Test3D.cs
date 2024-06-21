using Godot;

namespace F00F;

public partial class Test3D : Game3D
{
    protected TestArena Arena => GetNode<TestArena>("Arena");

    protected override bool OnUnhandledKeyInput(InputEventKey e)
    {
        return base.OnUnhandledKeyInput(e)
            || this.Handle(e.IsActionPressed(MyInput.ToggleArena), ToggleArena)
            || this.Handle(e.IsActionPressed(MyInput.ToggleTarget), ToggleTarget);

        void ToggleArena()
            => Arena.Visible = !Arena.Visible;

        void ToggleTarget()
        {
            if (Camera.Target is null) return;
            Camera.Target.Visible = !Camera.Target.Visible;
        }
    }
}
