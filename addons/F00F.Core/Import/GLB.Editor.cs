#if TOOLS
using Godot;

namespace F00F;

public static partial class GLB
{
    public static void OnEditorSave(CollisionObject3D root)
    {
        if (root is RigidBody3D body)
        {
            Editor.DoPreSaveReset(body, RigidBody3D.PropertyName.Mass, 1);
            Editor.DoPreSaveReset(body, RigidBody3D.PropertyName.Freeze);
        }

        Editor.DoPreSaveResetMeta(root, GLB.Aabb, GLB.Mass);
        Editor.DoPreSaveResetOwner(root, where: IsPart);
    }
}
#endif
