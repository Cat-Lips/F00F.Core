using Godot;

namespace F00F.NEW;

internal interface INetworkCommon
{
    bool Exists => false;
    StringName Name { get; }
}
