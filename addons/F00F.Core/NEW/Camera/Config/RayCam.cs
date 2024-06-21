using Godot;

namespace F00F;

[Tool, GlobalClass]
public partial class RayCam : CustomResource
{
    protected internal virtual bool Toggle()
        => MyInput.IsActionJustPressed(MyInput.Toggle);

    protected internal virtual bool Select()
        => MyInput.IsActionJustPressed(MyInput.Select);
}
