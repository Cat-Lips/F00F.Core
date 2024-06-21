#if TOOLS
using Godot;
using Godot.Collections;

namespace F00F;

public partial class TestArena
{
    public sealed override void _ValidateProperty(Dictionary property)
        => Editor.SetDisplayOnly(property, PropertyName.Config);

    public sealed override void _Notification(int what)
    {
        if (Editor.OnPreSave(what))
        {
            Editor.DoPreSaveReset(Stunts, Node3D.PropertyName.Scale, Vector3.One);
            Editor.DoPreSaveReset(Bounds, WorldBounds.PropertyName.Size, WorldBounds.Default.Size);
            Editor.DoPreSaveReset(FloorPlane, PlaneMesh.PropertyName.Size, Vector2.One * 2);
            Editor.DoPreSaveReset(FloorShape, CollisionShape3D.PropertyName.Shape);
            Editor.DoPreSaveReset(LoopMesh, MeshInstance3D.PropertyName.Mesh);
            Editor.DoPreSaveReset(LoopShape, CollisionShape3D.PropertyName.Shape);
            return;
        }

        if (Editor.OnPostSave(what))
            Editor.DoPostSaveRestore();
    }
}
#endif
