using Godot;

namespace F00F;

public interface IPlayer : ITarget, IActive
{
    Node3D Node => (Node3D)this;
    PhysicsBody3D Body => (PhysicsBody3D)this;
    CollisionObject3D Collider => (CollisionObject3D)this;

    PlayerInput Input { get; set; }
    ModelConfig Model { get; set; }
    PlayerConfig Config { get; set; }

    StringName Name { get; }

    bool Grounded { get; }
    Vector3 Velocity { get; }
}
