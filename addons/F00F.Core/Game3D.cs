using Godot;

namespace F00F;

[Tool]
public partial class Game3D : GameEnv
{
    protected Cam3D Camera => field ??= (Cam3D)GetNode("Camera");
}
