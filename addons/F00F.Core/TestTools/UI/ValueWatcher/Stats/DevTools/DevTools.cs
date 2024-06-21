using Godot;

namespace F00F;

[Tool]
public partial class DevTools : Stats
{
    public sealed override void _UnhandledKeyInput(InputEvent e)
    {
        if (this.Handle(MyInput.IsActionJustPressed(MyInput.Show), () => Visible = !Visible)) return;
    }
}
