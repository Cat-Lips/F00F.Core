using System.Collections.Generic;
using System.Linq;
using Godot;

namespace F00F;

using ShapeLookup = Dictionary<int, CollisionShape3D>;

public static class ColliderExtensions
{
    public static ShapeLookup IndexShapes(this CollisionObject3D source, IEnumerable<CollisionShape3D> shapes = null)
        => (shapes ?? source.GetChildren<CollisionShape3D>()).ToDictionary(x => x.GetIndex(includeInternal: true));

    public static CollisionShape3D GetShape(this CollisionObject3D source, long localShapeIndex)
        => source.ShapeOwnerGetOwner(source.ShapeFindOwner((int)localShapeIndex)) as CollisionShape3D;
}
