using Godot;

namespace F00F;

[Tool]
public partial class Game3D : GameEnv
{
    protected Camera3D Camera => field ??= (Camera3D)GetNode("Camera");
}
