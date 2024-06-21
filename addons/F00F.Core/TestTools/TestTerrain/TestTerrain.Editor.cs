#if TOOLS
using Godot;
using Godot.Collections;

namespace F00F;

public partial class TestTerrain
{
    public override void _ValidateProperty(Dictionary property)
    {
        if (this.IsEditedSceneRoot())
        {
            Editor.SetDisplayOnly(property, PropertyName.HeightMap);
            Editor.SetDisplayOnly(property, PropertyName.ColorMap);
            Editor.SetDisplayOnly(property, PropertyName.Gradient);
            Editor.SetDisplayOnly(property, PropertyName.ShapeType);
            Editor.SetDisplayOnly(property, PropertyName.MeshType);
            Editor.SetDisplayOnly(property, PropertyName.Amplitude);
        }
    }

    public sealed override void _Notification(int what)
    {
        if (Editor.OnPreSave(what))
        {
            Editor.DoPreSaveReset(Mesh, MeshInstance3D.PropertyName.Mesh);
            Editor.DoPreSaveReset(Shape, CollisionShape3D.PropertyName.Shape);
            Editor.DoPreSaveReset(Bounds, WorldBounds.PropertyName.Size, WorldBounds.Default.Size);
            return;
        }

        if (Editor.OnPostSave(what))
            Editor.DoPostSaveRestore();
    }
}
#endif
