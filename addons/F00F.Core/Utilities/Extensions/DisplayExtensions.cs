using Godot;

namespace F00F;

public static class DisplayExtensions
{
    public static bool MouseOver(this Control source)
        => new Rect2(default, source.GetSize()).HasPoint(source.GetLocalMousePosition());

    public static bool MouseOver(this Popup source)
        => new Rect2(default, source.GetSize()).HasPoint(source.GetMousePosition());

    public static Control GetRoot(this Control source)
    {
        if (source.TopLevel) return source;
        var parent = source.GetParentOrNull<Control>();
        return parent is null ? source : parent.GetRoot();
    }

    public static void FitToRect(this Camera2D camera, in Rect2I rect)
    {
        if (Editor.IsEditor) return;

        camera.Position = rect.GetCenter();
        camera.Zoom = rect.Size / GetEditorRect().Size;
    }

    public static void ZoomToView(this Camera2D camera)
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
