#if TOOLS
using Godot.Collections;

namespace F00F;

public partial class ModelConfig
{
    public sealed override void _ValidateProperty(Dictionary property)
    {
        if (Editor.Show(property, PropertyName.Scene, Source is null || Source.IsEmpty)) return;
        if (Editor.Show(property, PropertyName.Source, Scene is null)) return;
        if (Editor.Hide(property, PropertyName.Rotation, Scene is null)) return;
        if (Editor.Hide(property, PropertyName.OnSourceUpdated, Source is null)) return;
    }
}
#endif
