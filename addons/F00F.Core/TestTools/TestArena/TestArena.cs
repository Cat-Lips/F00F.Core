using Godot;

namespace F00F;

[Tool]
public partial class TestArena : Node3D
{
    public Marker3D SpawnPoint => field ??= GetNode<Marker3D>("SpawnPoint");
}
