using Godot;

namespace F00F.NEW;

[Tool]
public partial class Game3D : Game
{
    protected Camera Camera => field ??= (Camera)GetNode("Camera");
}
