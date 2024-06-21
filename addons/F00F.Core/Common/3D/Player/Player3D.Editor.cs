#if TOOLS
namespace F00F;

public partial class Player3D
{
    protected virtual void OnEditorSave() { }
    public sealed override void _Notification(int what)
    {
        if (Editor.OnPreSave(what))
        {
            if (this.IsEditedSceneRoot())
            {
                Editor.DoPreSaveResetField(this, PropertyName.Input);
                Editor.DoPreSaveResetField(this, PropertyName.Model);
                Editor.DoPreSaveResetField(this, PropertyName.Config);
            }

            GLB.OnEditorSave(this);
            OnEditorSave();
            return;
        }

        if (Editor.OnPostSave(what))
            Editor.DoPostSaveRestore();
    }
}
#endif
