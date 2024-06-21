#if TOOLS
using Godot;

namespace F00F.NEW;

public partial class Test3D
{
    protected sealed override void OnEditorSave()
    {
        base.OnEditorSave();
        Editor.DoPreSaveReset(Camera, Node3D.PropertyName.Transform, Transform3D.Identity);
    }
}
#endif
