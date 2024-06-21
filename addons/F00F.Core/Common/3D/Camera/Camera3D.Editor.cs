#if TOOLS
using Godot.Collections;

namespace F00F;

public partial class Camera3D
{
    public sealed override void _ValidateProperty(Dictionary property)
    {
        if (this.IsEditedSceneRoot())
        {
            if (Editor.SetDisplayOnly(property, PropertyName.Input)) return;
            if (Editor.SetDisplayOnly(property, PropertyName.Config)) return;
            if (Editor.SetDisplayOnly(property, PropertyName.Target)) return;
            if (Editor.SetDisplayOnly(property, PropertyName.OrbitMode)) return;
            if (Editor.SetDisplayOnly(property, PropertyName.SelectMode)) return;
        }
    }
}
#endif
