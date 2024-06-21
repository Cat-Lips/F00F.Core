using Godot;

namespace F00F;

[Tool]
public partial class Game2D : CanvasLayer
{
    public Game2D()
        => Layer = Const.CanvasLayer.Game2D;

    protected virtual void OnReady() { }
    public sealed override void _Ready()
    {
        FollowViewportEnabled = false;
        //Camera.StretchToFitScreen();
        OnReady();
    }

    public sealed override void _Input(InputEvent e)
    {
        if (e is InputEventMouseButton mouse)
        {
            //GD.Print(Camera.Zoom);

            if (mouse.ButtonIndex is MouseButton.WheelUp)
            {
                //Camera.Zoom *= 2f;
                FollowViewportScale += 2f;
            }

            else if (mouse.ButtonIndex is MouseButton.WheelDown)
            {
                //Camera.Zoom *= .5f;
                FollowViewportScale *= .5f;
            }
        }
    }
}
