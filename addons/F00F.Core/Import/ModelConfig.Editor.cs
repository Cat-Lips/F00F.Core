#if TOOLS
using Godot.Collections;

namespace F00F;

public partial class ModelConfig
{
    public sealed override void _ValidateProperty(Dictionary property)
    {
        if (Editor.Show(property, PropertyName.Scene, Source is null || Source.IsEmpty)) return;
        if (Editor.Show(property, PropertyName.Source, Scene is null)) return;
        if (Editor.Show(property, PropertyName.Rotation, Scene is not null)) return;
        if (Editor.Show(property, PropertyName.ScaleMultiplier, Scene is not null)) return;
        if (Editor.Show(property, PropertyName.OnSourceUpdated, Source is not null && !Source.IsEmpty)) return;
    }
}
#endif
