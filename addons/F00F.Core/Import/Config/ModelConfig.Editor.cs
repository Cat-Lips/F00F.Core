#if TOOLS
using Godot.Collections;

namespace F00F;

public partial class ModelConfig
{
    protected virtual bool ValidateProperty(Dictionary property) => false;
    public sealed override void _ValidateProperty(Dictionary property)
    {
        if (ValidateProperty(property)) return;

        var HasSource = !Source.IsNullOrEmpty();
        if (Editor.Hide(property, PropertyName.Scene, HasSource)) return;
        if (Editor.Show(property, PropertyName.OnSourceUpdated, HasSource)) return;

        var HasScene = Scene is not null;
        if (Editor.Hide(property, PropertyName.Source, HasScene)) return;
        if (Editor.Show(property, PropertyName.Rotation, HasScene)) return;
        if (Editor.Show(property, PropertyName.MeshScaleMultiplier, HasScene)) return;

        var HasSceneOrSource = HasScene || HasSource;
        if (Editor.Show(property, PropertyName.PartsShape, HasSceneOrSource)) return;
        if (Editor.Show(property, PropertyName.BoundingShape, HasSceneOrSource)) return;
        if (Editor.Show(property, PropertyName.MassMultiplier, HasSceneOrSource)) return;
        if (Editor.Show(property, PropertyName.ShapeScaleMultiplier, HasSceneOrSource)) return;
    }
}
#endif
