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
            if (Editor.SetDisplayOnly(property, PropertyName.SelectMode)) return;
        }
    }

    public void SkipSave()
    {
        Editor.DoPreSaveResetField(this, PropertyName.Input);
        Editor.DoPreSaveResetField(this, PropertyName.Config);
        Editor.DoPreSaveResetField(this, PropertyName.Target);
        Editor.DoPreSaveResetField(this, PropertyName.SelectMode);
    }
}
#endif
