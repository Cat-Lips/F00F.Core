using Godot;

namespace F00F;

public interface ITarget
{
    public Node3D Target => (Node3D)this;
}
