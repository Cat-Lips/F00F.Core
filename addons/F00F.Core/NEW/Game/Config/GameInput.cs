using Godot;

namespace F00F;

[Tool, GlobalClass]
public partial class GameInput : CustomResource
{
    protected internal virtual bool QuickHelp(InputEvent e)
        => e.IsActionJustPressed(MyInput.QuickHelp);

    protected internal virtual bool QuickExit(InputEvent e)
        => e.IsActionJustPressed(MyInput.QuickExit);
}
