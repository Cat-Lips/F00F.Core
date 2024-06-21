using Godot;

namespace F00F.NEW;

[Tool]
internal partial class DirectNetwork : Node, INetworkProvider
{
    public bool Exists => true; // Always
}
