using Godot;

namespace F00F;

[Tool]
public partial class Game2D : Game
{
    protected Camera2D Camera => field ??= (Camera2D)GetNode("Camera");
}
