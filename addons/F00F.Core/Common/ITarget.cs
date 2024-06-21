using Godot;

namespace F00F;

public interface ITarget
{
    Node3D Target => (Node3D)this;
}
