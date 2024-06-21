using System.Linq;
using Godot;

namespace F00F;

public static class MeshExtensions_Node
{
    public static Aabb BB(this Node source) => source.GetMeshAabb();
    public static Aabb Bounds(this Node source) => source.GetMeshAabb();
    public static Aabb GetBounds(this Node source) => source.GetMeshAabb();
    public static Aabb GetMeshAabb(this Node source)
    {
        return source.RecurseVisible<MeshInstance3D>(self: true)
            .Select(x => x.GetTransformedAabb())
            .AggregateOrDefault((a, b) => a.Merge(b));
    }

    public static Aabb GetTransformedAabb(this MeshInstance3D source)
        => source.GetAabb().TransformedBy(source.GlobalTransform);
}
