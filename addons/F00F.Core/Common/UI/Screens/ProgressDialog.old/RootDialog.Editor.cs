#if TOOLS
using Godot;

namespace F00F;

public partial class RootDialog
{
    protected virtual void OnEditorSave() { }
    public sealed override void _Notification(int what)
    {
        if (Editor.OnPreSave(what))
        {
            Editor.DoPreSaveReset(this, PropertyName.Size, new Vector2I(100, 100));
            Editor.DoPreSaveReset(this, PropertyName.Visible);
            OnEditorSave();
            return;
        }

        if (Editor.OnPostSave(what))
            Editor.DoPostSaveRestore();
    }
}
#endif
