#if TOOLS
using Godot;
using Godot.Collections;

namespace F00F;

public partial class TestTerrain
{
    public sealed override void _ValidateProperty(Dictionary property)
        => Editor.SetDisplayOnly(property, PropertyName.Config);

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
