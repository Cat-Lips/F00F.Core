#if TOOLS
using Godot.Collections;

namespace F00F;

public partial class GUI
{
    public sealed override void _ValidateProperty(Dictionary property)
    {
        //if (this.IsEditedSceneRoot())
        //    if (Editor.SetDisplayOnly(property, PropertyName.MainMenu)) return;
    }

    public sealed override void _Notification(int what)
    {
        App.NotifyQuit(this, what);

        if (Editor.OnPreSave(what))
        {
            Editor.DoPreSaveReset(this, PropertyName.Layer, 1);
            return;
        }

        if (Editor.OnPostSave(what))
            Editor.DoPostSaveRestore();
    }
}
#endif
