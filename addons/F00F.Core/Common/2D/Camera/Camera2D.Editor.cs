#if TOOLS
using Godot.Collections;

namespace F00F;

public partial class Camera2D
{
    public sealed override void _ValidateProperty(Dictionary property)
    {
        if (this.IsEditedSceneRoot())
        {
            if (Editor.SetDisplayOnly(property, PropertyName.Config)) return;
        }
    }
}
#endif
