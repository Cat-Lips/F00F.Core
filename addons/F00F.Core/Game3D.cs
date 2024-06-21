using Godot;

namespace F00F;

[Tool]
public partial class Game3D : GameEnv
{
    protected Camera3D Camera => field ??= GetNode<Camera3D>("Camera");
}
