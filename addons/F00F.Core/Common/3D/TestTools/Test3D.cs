using Godot;

namespace F00F
{
    [Tool]
    public partial class Test3D : Game3D
    {
        protected TestArena Arena => GetNode<TestArena>("Arena");

        protected override bool OnUnhandledKeyInput(InputEvent e)
        {
            return base.OnUnhandledKeyInput(e)
                || this.Handle(Input.IsActionJustPressed(MyInput.ToggleArena), ToggleArena)
                || this.Handle(Input.IsActionJustPressed(MyInput.ToggleTarget), ToggleTarget);

            void ToggleArena()
                => Arena.Visible = !Arena.Visible;

            void ToggleTarget()
            {
                if (Camera.Target is null) return;
                Camera.Target.Visible = !Camera.Target.Visible;
            }
        }
    }
}
