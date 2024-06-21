using Godot;

namespace F00F;

public static partial class GLB
{
    internal static void RemoveMeta(Node root)
    {
        if (root is null) return;
        if (root is RigidBody3D body) body.Mass = 1;
        else root.RemoveMeta(Mass);
        root.RemoveMeta(Aabb);
    }
}
