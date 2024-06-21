#if TOOLS
using Godot;

namespace F00F;

[Tool]
public partial class Game3D
{
    public override void _Notification(int what)
    {
        if (Editor.OnPreSave(what))
        {
            if (this.IsEditedSceneRoot())
                Editor.DoPreSaveReset(Camera, Camera.PropertyName.Config);
            return;
        }

        if (Editor.OnPostSave(what))
            Editor.DoPostSaveRestore();
    }
}
#endif
