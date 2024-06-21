using System.Diagnostics;
using Godot;
using Godot.Collections;
using PointCollision = (Godot.CollisionObject3D Body, Godot.Vector3 Point, Godot.Vector3 Normal);
using ShapeCollision = (Godot.CollisionObject3D Body, Godot.CollisionShape3D Shape, Godot.Vector3 Point, Godot.Vector3 Normal);

namespace F00F;

public static class RayCastExtensions
{
    public static bool RayCastHit(this Node3D node, float distance) => node.RayCastHit(node.Position, node.Transform.Basis.Z, distance);
    public static bool RayCastHit(this Node3D node, in Vector3 direction, float distance) => node.RayCastHit(node.Position, direction, distance);
    public static bool RayCastHit(this Node3D node, in Vector3 rayStart, in Vector3 direction, float distance)
        => RayCollision(node, rayStart, direction, distance, out var _);

    public static Node3D RayCastHitBody(this Node3D node, float distance) => node.RayCastHitBody(node.Position, node.Transform.Basis.Z, distance);
    public static Node3D RayCastHitBody(this Node3D node, in Vector3 direction, float distance) => node.RayCastHitBody(node.Position, direction, distance);
    public static Node3D RayCastHitBody(this Node3D node, in Vector3 rayStart, in Vector3 direction, float distance)
    {
        return RayCollision(node, rayStart, direction, distance, out var result)
            ? (Node3D)result["collider"].AsGodotObject()
            : null;
    }

    public static PointCollision RayCastHitPoint(this Node3D node, float distance) => node.RayCastHitPoint(node.Position, node.Transform.Basis.Z, distance);
    public static PointCollision RayCastHitPoint(this Node3D node, in Vector3 direction, float distance) => node.RayCastHitPoint(node.Position, direction, distance);
    public static PointCollision RayCastHitPoint(this Node3D node, in Vector3 rayStart, in Vector3 direction, float distance)
    {
        if (RayCollision(node, rayStart, direction, distance, out var result))
        {
            var surfaceNormal = result["normal"].AsVector3();
            var intersectPoint = result["position"].AsVector3();
            var collisionObject = (CollisionObject3D)result["collider"].AsGodotObject();

            return (collisionObject, intersectPoint, surfaceNormal);
        }

        return default;
    }

    public static ShapeCollision RayCastHitShape(this Node3D node, float distance) => node.RayCastHitShape(node.Position, node.Transform.Basis.Z, distance);
    public static ShapeCollision RayCastHitShape(this Node3D node, in Vector3 direction, float distance) => node.RayCastHitShape(node.Position, direction, distance);
    public static ShapeCollision RayCastHitShape(this Node3D node, in Vector3 rayStart, in Vector3 direction, float distance)
    {
        if (RayCollision(node, rayStart, direction, distance, out var result))
        {
            var surfaceNormal = result["normal"].AsVector3();
            var intersectPoint = result["position"].AsVector3();
            var collisionObject = (CollisionObject3D)result["collider"].AsGodotObject();

            var shapeIndex = result["shape"].AsInt32();
            var shapeOwner = collisionObject.ShapeFindOwner(shapeIndex);
            var collisionShape = (CollisionShape3D)collisionObject.ShapeOwnerGetOwner(shapeOwner);

            return (collisionObject, collisionShape, intersectPoint, surfaceNormal);
        }

        return default;
    }

    private static bool RayCollision(Node3D node, in Vector3 rayStart, in Vector3 direction, float distance, out Dictionary result)
    {
        Debug.Assert(Engine.IsInPhysicsFrame());

        var rayEnd = rayStart + direction * distance;
        var query = PhysicsRayQueryParameters3D.Create(rayStart, rayEnd,
            exclude: node is CollisionObject3D collider ? new() { collider.GetRid() } : null);
        result = node.WorldSpace().IntersectRay(query);
        return result.Count is not 0;
    }

    private static PhysicsDirectSpaceState3D _worldSpace;
    private static PhysicsDirectSpaceState3D WorldSpace(this Node3D node)
        => _worldSpace ??= node.GetWorld3D().DirectSpaceState;
}
