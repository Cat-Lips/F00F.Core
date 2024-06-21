using System;
using System.Linq;
using Godot;

namespace F00F
{
    public static class ShapeExtensions_Aabb
    {
        public static Aabb GetAabb(this CollisionObject3D source, bool reset = false, bool cache = true)
        {
            return GetAabbFromCache().TransformedBy(source.Transform);

            Aabb GetAabbFromCache()
            {
                if (!reset && source.HasMeta(nameof(Aabb)))
                    return (Aabb)source.GetMeta(nameof(Aabb));

                var bb = GetSourceAabb(source);

                if (cache)
                    source.SetMeta(nameof(Aabb), bb);

                return bb;

                static Aabb GetSourceAabb(CollisionObject3D source)
                {
                    return source.GetChildren<CollisionShape3D>().Select(GetShapeAabb).Aggregate((a, b) => a.Merge(b));

                    static Aabb GetShapeAabb(CollisionShape3D source)
                        => source.Shape.GetAabb().TransformedBy(source.Transform);
                }
            }
        }

        public static Aabb GetAabb(this Shape3D shape)
        {
            return shape is BoxShape3D box ? box.GetAabb()
                : shape is SphereShape3D sphere ? sphere.GetAabb()
                : shape is CapsuleShape3D capsule ? capsule.GetAabb()
                : shape is CylinderShape3D cylinder ? cylinder.GetAabb()
                : shape is ConvexPolygonShape3D convex ? convex.GetAabb()
                : shape is ConcavePolygonShape3D concave ? concave.GetAabb()
                : throw new NotImplementedException($"{shape.GetType().Name}.GetAabb()");
        }

        public static Aabb GetAabb(this BoxShape3D shape) => new() { Size = shape.Size };
        public static Aabb GetAabb(this SphereShape3D shape) => new() { Size = Vector3.One * (shape.Radius * 2) };
        public static Aabb GetAabb(this CapsuleShape3D shape) => new() { Size = new(shape.Radius * 2, shape.Height, shape.Radius * 2) };
        public static Aabb GetAabb(this CylinderShape3D shape) => new() { Size = new(shape.Radius * 2, shape.Height, shape.Radius * 2) };
        public static Aabb GetAabb(this ConvexPolygonShape3D shape) => shape.Points.Skip(1).Aggregate(new Aabb(shape.Points.First(), default), (a, b) => a.Expand(b));
        public static Aabb GetAabb(this ConcavePolygonShape3D shape) => throw new NotImplementedException("Slow - Don't use this for dynamic bodies");
    }
}
