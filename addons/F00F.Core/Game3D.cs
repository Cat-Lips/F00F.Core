namespace F00F;

public partial class Game3D : Game
{
    protected Camera3D Camera => field ??= GetNode<Camera3D>("Camera");
}
