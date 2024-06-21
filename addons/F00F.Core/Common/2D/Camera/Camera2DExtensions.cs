using Godot;

namespace F00F;

public static class Camera2DExtensions
{
    public static void FitToWorld(this Godot.Camera2D source, in Rect2I worldRect, float min = 1, float max = 10)
    {
        SetZoom(worldRect);
        SetLimits(worldRect);

        void SetZoom(in Rect2I worldRect)
        {
            var viewRect = source.GetViewportRect();
            var viewAspect = viewRect.Size.X / viewRect.Size.Y;
            var worldAspect = worldRect.Size.X / (float)worldRect.Size.Y;

            var zoom = viewAspect > worldAspect
                ? viewRect.Size.X / worldRect.Size.X
                : viewRect.Size.Y / worldRect.Size.Y;

            source.Zoom = (Vector2.One * zoom).Clamp(min, max);
        }

        void SetLimits(in Rect2I worldRect)
        {
            source.LimitTop = worldRect.Position.Y;
            source.LimitLeft = worldRect.Position.X;
            source.LimitRight = worldRect.End.X;
            source.LimitBottom = worldRect.End.Y;

            //source.LimitSmoothed = true;
            //source.PositionSmoothingEnabled = true;
        }
    }

    public static Vector2 GetZoomSize(this Godot.Camera2D source)
    {
        var zoom = source.Zoom;
        var view = source.GetViewportRect().Size;
        return new(view.X / zoom.X, view.Y / zoom.Y);
    }
}
