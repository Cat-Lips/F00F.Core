using System;
using System.Linq;
using Godot;

namespace F00F;

public static class ShapeExtensions
{
    public static Aabb CurAabb(this CollisionObject3D self) => self.GetAabb(reset: true, cache: false);
    public static Aabb GetAabb(this CollisionObject3D self, bool reset = false, bool cache = true)
    {
        return self.GetCachedAabb(reset, cache, GetAabb).TransformedBy(self.Transform);

        Aabb GetAabb()
        {
            return self.GetChildren<CollisionShape3D>()
                .Select(x => x.GetAabb(reset, cache))
                .DefaultIfEmpty()
                .Aggregate((a, b) => a.Merge(b));
        }
    }

    public static Aabb CurAabb(this CollisionShape3D self) => self.GetAabb(reset: true, cache: false);
    public static Aabb GetAabb(this CollisionShape3D self, bool reset = false, bool cache = true)
    {
        return self.GetCachedAabb(reset, cache, GetAabb).TransformedBy(self.Transform);

        Aabb GetAabb()
            => self.Shape.GetAabb();
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

    private static Aabb GetCachedAabb(this GodotObject self, bool reset, bool cache, Func<Aabb> GetAabb)
    {
        if (!reset && self.HasMeta(GLB.Aabb))
            return (Aabb)self.GetMeta(GLB.Aabb);

        var bb = GetAabb();

        if (cache)
            self.SetMeta(GLB.Aabb, bb);

        return bb;
    }
}
