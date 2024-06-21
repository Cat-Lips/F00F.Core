namespace F00F;

using Camera = Godot.Camera3D;

public static class WorldBoundsExtensions
{
    public static void InitTopView(this WorldBounds self, Camera camera = null)
    {
        camera ??= self.GetViewport().GetCamera3D();

        SetSize();
        self.GetViewport().SizeChanged += SetSize;

        void SetSize()
        {
            var screenSize = self.GetViewport().GetVisibleRect().Size;
            var screenSizeAtOrigin = camera.ProjectPosition(screenSize, camera.Position.Y);
            self.Size = (screenSizeAtOrigin * 2).XZ();
        }
    }
}
