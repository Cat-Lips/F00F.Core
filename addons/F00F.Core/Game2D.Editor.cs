#if TOOLS
namespace F00F;

public partial class Game2D
{
    protected virtual void OnEditorSave() { }
    public sealed override void _Notification(int what)
    {
        if (Editor.OnPreSave(what))
        {
            if (this.IsEditedSceneRoot())
                Editor.DoPreSaveResetField(Camera, Camera2D.PropertyName.Config);
            OnEditorSave();
            return;
        }

        if (Editor.OnPostSave(what))
            Editor.DoPostSaveRestore();
    }
}
#endif
