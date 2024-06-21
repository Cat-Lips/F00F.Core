using Godot;

namespace F00F;

public interface IPlayer : ITarget, IActive
{
    Node3D Node => (Node3D)this;
    PhysicsBody3D Body => (PhysicsBody3D)this;
    CollisionObject3D Collider => (CollisionObject3D)this;

    [Export] PlayerInput Input { get; set; }
    [Export] PlayerModel Model { get; set; }
    [Export] PlayerConfig Config { get; set; }

    StringName Name { get; }

    bool Grounded { get; }
    Vector3 Velocity { get; }
}
