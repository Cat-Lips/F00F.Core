using Godot;

namespace F00F;

public interface ITarget
{
    Node3D Node => (Node3D)this;
    Node3D Target => (Node3D)this;
    PhysicsBody3D Body => (PhysicsBody3D)this;
    CollisionObject3D Collider => (CollisionObject3D)this;
}
