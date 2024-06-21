using Godot;

namespace F00F;

[Tool]
public partial class Game2D : Game
{
    protected Cam2D Camera => field ??= (Cam2D)GetNode("Camera");
}
