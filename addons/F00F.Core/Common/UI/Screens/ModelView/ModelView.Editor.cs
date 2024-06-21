#if TOOLS
using Godot;

namespace F00F;

public partial class ModelView
{
    public sealed override void _Notification(int what)
    {
        if (Editor.OnPreSave(what))
        {
            Editor.DoPreSaveReset(View, SubViewport.PropertyName.Size);
            Editor.DoPreSaveReset(Camera, Camera3D.PropertyName.Position);
            return;
        }

        if (Editor.OnPostSave(what))
            Editor.DoPostSaveRestore();
    }
}
#endif
