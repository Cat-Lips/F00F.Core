using Godot;

namespace F00F.NEW;

[Tool]
internal partial class LocalNetwork : Node, INetworkProvider
{
    public bool Exists => true; // Always
}
