using Godot;

namespace F00F;

public static class DisplayExtensions
{
    public static void FitToRect(this Cam2D camera, in Rect2I rect)
    {
        if (Editor.IsEditor) return;

        camera.Position = rect.GetCenter();
        camera.Zoom = rect.Size / GetEditorRect().Size;
    }

    public static void ZoomToView(this Cam2D camera)
    {
        if (Editor.IsEditor) return;

        var view = View();
        ZoomToView();
        view.SizeChanged += ZoomToView;

        Viewport View()
            => camera.CustomViewport as Viewport ?? camera.GetViewport();

        void ZoomToView()
            => camera.Zoom = view.GetVisibleRect().Size / GetEditorRect().Size;
    }

    public static Window GetMainWindow(this Node source)
        => source.GetTree().Root;

    public static Rect2 GetDisplayRect(this Node source)
        => Editor.IsEditor ? GetEditorRect() : source.GetScreenRect();

    private static Rect2 GetEditorRect() => new(0, 0,
        (float)ProjectSettings.GetSettingWithOverride("display/window/size/viewport_width"),
        (float)ProjectSettings.GetSettingWithOverride("display/window/size/viewport_height"));

    private static Rect2 GetScreenRect(this Node source)
        => source.GetViewport().GetVisibleRect();
}
