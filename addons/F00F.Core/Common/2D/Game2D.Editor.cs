#if TOOLS
namespace F00F;

public partial class Game2D
{
    protected virtual void OnEditorSave() { }
    public sealed override void _Notification(int what)
    {
        if (Editor.OnPreSave(what))
        {
            Editor.DoPreSaveReset(this, PropertyName.Layer, 1);
            //Editor.DoPreSaveReset(Camera, Camera2D.PropertyName.Zoom, Vector2.One);
            OnEditorSave();
            return;
        }

        if (Editor.OnPostSave(what))
            Editor.DoPostSaveRestore();
    }
}
#endif
