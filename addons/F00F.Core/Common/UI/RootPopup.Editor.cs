#if TOOLS
namespace F00F;

public partial class RootPopup
{
    protected virtual void OnEditorSave() { }
    public sealed override void _Notification(int what)
    {
        if (Editor.OnPreSave(what))
        {
            Editor.DoPreSaveReset(this, PropertyName.Layer, 1);
            Editor.DoPreSaveReset(this, PropertyName.Visible, true);
            OnEditorSave();
            return;
        }

        if (Editor.OnPostSave(what))
            Editor.DoPostSaveRestore();
    }
}
#endif
