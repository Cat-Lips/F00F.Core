using Godot;

namespace F00F;

[Tool]
public partial class Game2D : CanvasLayer
{
    public Game2D()
        => Layer = Const.CanvasLayer.Game2D;
}
