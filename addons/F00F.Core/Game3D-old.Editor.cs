#if TOOLS
namespace F00F;

public partial class Game3Dold
{
    protected virtual void OnEditorSave() { }
    public sealed override void _Notification(int what)
    {
        if (Editor.OnPreSave(what))
        {
            if (this.IsEditedSceneRoot())
                Editor.DoPreSaveReset(Camera, Camera3D.PropertyName.Config);
            OnEditorSave();
            return;
        }

        if (Editor.OnPostSave(what))
            Editor.DoPostSaveRestore();
    }
}
#endif
