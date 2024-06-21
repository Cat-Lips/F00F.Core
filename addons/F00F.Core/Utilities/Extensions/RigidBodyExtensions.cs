using System;
using System.Linq;
using Godot;

namespace F00F;

public static class RigidBodyExtensions
{
    public static void InitCollisions(this RigidBody3D source, int? contactCount = null)
    {
        source.ContactMonitor = true;
        source.MaxContactsReported = contactCount ?? (source.GetChildren<CollisionShape3D>().Count() * 10);
    }

    public static bool IsColliding(this RigidBody3D source) => source.GetCollidingBodies().Count > 0;
    public static bool IsCollidingWith(this RigidBody3D source, CollisionObject3D body) => source.GetCollidingBodies().Contains(body);
    public static bool IsCollidingWith<T>(this RigidBody3D source) where T : CollisionObject3D => source.GetCollidingBodies().Any(x => x.GetType() == typeof(T));

    public static Vector3 GetPointVelocity(this RigidBody3D source, in Vector3 point)
        => source.LinearVelocity + source.AngularVelocity.Cross(point - source.GlobalPosition);

    public static bool IsMovingTowards(this RigidBody3D source, in Vector3 target)
        => source.IsMovingTowards(target, source.LinearVelocity);

    public static bool IsMovingAwayFrom(this RigidBody3D source, in Vector3 target)
        => source.IsMovingAwayFrom(target, source.LinearVelocity);
}
