using Godot;

namespace F00F;

using Camera = Godot.Camera3D;

public static class WorldBoundsExtensions
{
    public static void InitTopView(this WorldBounds self, Camera camera = null)
    {
        camera ??= self.GetCameraOrNull();
        if (camera is null) return;

        SetSize();
        self.GetViewport().SizeChanged += SetSize;

        void SetSize()
        {
            var screenSize = self.GetDisplayRect().Size;
            var screenSizeAtOrigin = camera.ProjectPosition(screenSize, camera.Position.Y);
            self.Size = (screenSizeAtOrigin * 2).XZ();
        }
    }

    public static IBounds TopView(this WorldBounds self, float y, Camera camera = null)
    {
        if (y is 0) return self;

        camera ??= self.GetCameraOrNull();
        if (camera is null) return self;

        var depth = camera.Position.Y - y;
        var (left, _, front) = camera.ProjectPosition(Vector2.Zero, depth);
        return new Bounds(left, -front, -left, front);
    }

    private static Camera GetCameraOrNull(this WorldBounds world)
        => world.GetViewport()?.GetCamera3D();
}
