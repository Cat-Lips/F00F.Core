using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;

namespace F00F;

using RigidBodyContactData = IDictionary<CollisionShape3D, RigidBodyCombinedContacts>;
using RigidBodyContacts = ILookup<CollisionShape3D, RigidBodyContactLookup>;
using ShapeLookup = Dictionary<int, CollisionShape3D>;

#region KinematicContact

public record KinematicContact(
    PhysicsBody3D Other,
    CollisionShape3D LocalShape,
    CollisionShape3D OtherShape,
    float ImpactAngle,
    Vector3 ImpactNormal,
    Vector3 ImpactPosition,
    Vector3 OtherBodyVelocity);

#endregion

#region RigidBodyContact

public record RigidBodyContact(
    PhysicsBody3D Other,
    int OtherShapeIdx,
    Vector3 OtherImpactPosition,
    Vector3 OtherPointVelocity,
    int ShapeIdx,
    Vector3 ImpactNormal,
    Vector3 ImpactPosition,
    Vector3 PointVelocity);

public record RigidBodyContactLookup(
    PhysicsBody3D Other,
    Vector3 OtherImpactPosition,
    Vector3 OtherPointVelocity,
    CollisionShape3D Shape,
    Vector3 ImpactNormal,
    Vector3 ImpactPosition,
    Vector3 PointVelocity);

public record RigidBodyCombinedContacts(
    Vector3 ImpactNormal,
    Vector3 ImpactPosition,
    Vector3 PointVelocity);

#endregion

public static class CollisionExtensions
{
    #region KinematicContact

    public static void Get(this KinematicCollision3D state, out float depth, out Vector3 travel, out Vector3 remainder)
    {
        depth = state.GetDepth();
        travel = state.GetTravel();
        remainder = state.GetRemainder();
    }

    public static IEnumerable<KinematicContact> GetCollisions(this KinematicCollision3D state, out float depth, out Vector3 travel, out Vector3 remainder, Vector3? up = null)
    {
        state.Get(out depth, out travel, out remainder);
        return state.GetCollisions(up);
    }

    public static IEnumerable<KinematicContact> GetCollisions(this KinematicCollision3D state, Vector3? up = null)
    {
        var count = state.GetCollisionCount();
        for (var i = 0; i < count; ++i)
        {
            yield return new(
                (PhysicsBody3D)state.GetCollider(i),
                (CollisionShape3D)state.GetLocalShape(),
                (CollisionShape3D)state.GetColliderShape(i),
                state.GetAngle(i, up),
                state.GetNormal(i),
                state.GetPosition(i),
                state.GetColliderVelocity(i));
        }
    }

    public static IEnumerable<KinematicContact> GetSlideCollisions(this CharacterBody3D self, Vector3? up = null)
    {
        return GetSlideCollisions().SelectMany(x => x);

        IEnumerable<IEnumerable<KinematicContact>> GetSlideCollisions()
        {
            var count = self.GetSlideCollisionCount();
            for (var i = 0; i < count; ++i)
            {
                var state = self.GetSlideCollision(i);
                yield return state.GetCollisions(up);
            }
        }
    }

    #endregion

    #region RigidBodyContact

    public static IEnumerable<RigidBodyContact> GetCollisions(this PhysicsDirectBodyState3D state)
    {
        var count = state.GetContactCount();
        for (var i = 0; i < count; ++i)
        {
            yield return new(
                (PhysicsBody3D)state.GetContactColliderObject(i),
                state.GetContactColliderShape(i),
                state.GetContactColliderPosition(i),
                state.GetContactColliderVelocityAtPosition(i),
                state.GetContactLocalShape(i),
                state.GetContactLocalNormal(i),
                state.GetContactLocalPosition(i),
                state.GetContactLocalVelocityAtPosition(i));
        }
    }

    public static RigidBodyContacts GetCollisions(this PhysicsDirectBodyState3D state, ShapeLookup shapes)
    {
        return Collisions().ToLookup(x => x.Shape);

        IEnumerable<RigidBodyContactLookup> Collisions()
        {
            var count = state.GetContactCount();
            for (var i = 0; i < count; ++i)
            {
                var shapeIdx = state.GetContactLocalShape(i);
                var shape = shapes.TryGet(shapeIdx);
                if (shape is null) continue;

                yield return new(
                    (PhysicsBody3D)state.GetContactColliderObject(i),
                    state.GetContactColliderPosition(i),
                    state.GetContactColliderVelocityAtPosition(i),
                    shape,
                    state.GetContactLocalNormal(i),
                    state.GetContactLocalPosition(i),
                    state.GetContactLocalVelocityAtPosition(i));
            }
        }
    }

    public static RigidBodyContactData GetCollisionData(this PhysicsDirectBodyState3D state, ShapeLookup shapes)
    {
        return Collisions()
            .ToLookup(x => x.Shape, x => x.Contacts)
            .ToDictionary(x => x.Key, x => x.Average());

        IEnumerable<(CollisionShape3D Shape, RigidBodyCombinedContacts Contacts)> Collisions()
        {
            var count = state.GetContactCount();
            for (var i = 0; i < count; ++i)
            {
                var shapeIdx = state.GetContactLocalShape(i);
                var shape = shapes.TryGet(shapeIdx);
                if (shape is null) continue;

                yield return (shape, new(
                    state.GetContactLocalNormal(i),
                    state.GetContactLocalPosition(i),
                    state.GetContactLocalVelocityAtPosition(i)));
            }
        }
    }

    private static RigidBodyCombinedContacts Average(this IEnumerable<RigidBodyCombinedContacts> contacts)
    {
        var count = 0;
        var normal = Vector3.Zero;
        var position = Vector3.Zero;
        var velocity = Vector3.Zero;

        foreach (var (_normal, _position, _velocity) in contacts)
        {
            ++count;
            normal += _normal;
            position += _position;
            velocity += _velocity;
        }

        Debug.Assert(count is not 0);
        return new(normal / count, position / count, velocity / count);
    }

    #endregion
}
