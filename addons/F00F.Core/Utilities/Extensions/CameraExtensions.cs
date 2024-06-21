using Godot;
using static Godot.Camera2D;

namespace F00F;

public static class CameraExtensions
{
    public static void SetTileMap(this Godot.Camera2D source, TileMapLayer tm)
    {
        if (source is Camera2D fCam)
            fCam.TileMap = tm;
    }

    public static void FitRect(this Camera2D source, in Rect2 worldRect)
    {
        var viewRect = source.GetViewportRect();

        var viewAspect = viewRect.Size.X / viewRect.Size.Y;
        var worldAspect = worldRect.Size.X / worldRect.Size.Y;

        var zoom = viewAspect > worldAspect
            ? viewRect.Size.X / worldRect.Size.X
            : viewRect.Size.Y / worldRect.Size.Y;

        source.Zoom = Vector2.One * zoom;
        source.Position = worldRect.Position;
        source.AnchorMode = AnchorModeEnum.FixedTopLeft;
    }
}
