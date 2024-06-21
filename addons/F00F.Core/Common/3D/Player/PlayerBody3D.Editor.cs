#if TOOLS
namespace F00F;

public partial class PlayerBody3D
{
    protected virtual void OnEditorSave() { }
    public sealed override void _Notification(int what)
    {
        if (Editor.OnPreSave(what))
        {
            if (this.IsEditedSceneRoot())
            {
                Editor.DoPreSaveReset(this, PropertyName.Input);
                Editor.DoPreSaveReset(this, PropertyName.Model);
                Editor.DoPreSaveReset(this, PropertyName.Config);
            }

            Editor.DoPreSaveReset(this, PropertyName.ContactMonitor);
            Editor.DoPreSaveReset(this, PropertyName.MaxContactsReported);

            GLB.OnEditorSave(this);
            OnEditorSave();
            return;
        }

        if (Editor.OnPostSave(what))
            Editor.DoPostSaveRestore();
    }
}
#endif
