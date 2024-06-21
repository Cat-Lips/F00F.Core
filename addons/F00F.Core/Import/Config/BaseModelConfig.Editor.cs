#if TOOLS
using Godot.Collections;

namespace F00F;

public partial class BaseModelConfig
{
    protected virtual bool ValidateProperty(Dictionary property) => false;
    public sealed override void _ValidateProperty(Dictionary property)
    {
        if (ValidateProperty(property)) return;

        var HasScene = Scene is not null;
        if (Editor.Show(property, PropertyName.Rotation, HasScene)) return;
        if (Editor.Show(property, PropertyName.MassMultiplier, HasScene)) return;
        if (Editor.Show(property, PropertyName.MeshScaleMultiplier, HasScene)) return;
        if (Editor.Show(property, PropertyName.ShapeScaleMultiplier, HasScene)) return;
    }
}
#endif
