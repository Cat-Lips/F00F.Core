using Godot;

namespace F00F;

[Tool]
public partial class Game2D : Game
{
    protected Camera2D Camera => field ??= GetNode<Camera2D>("Camera");
}
