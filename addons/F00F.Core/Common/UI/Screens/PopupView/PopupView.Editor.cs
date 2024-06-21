#if TOOLS
namespace F00F;

public partial class PopupView
{
    protected virtual void OnSave() { }
    protected virtual void OnSaveSelf() { }
    public sealed override void _Notification(int what)
    {
        if (Editor.OnPreSave(what))
        {
            if (this.IsEditedSceneRoot())
                OnSaveSelf();
            OnSave();
            return;
        }

        if (Editor.OnPostSave(what))
            Editor.DoPostSaveRestore();
    }
}
#endif
