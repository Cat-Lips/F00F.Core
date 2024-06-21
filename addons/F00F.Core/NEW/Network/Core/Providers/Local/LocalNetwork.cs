using System;
using Godot;

namespace F00F.NEW;

[Tool]
internal partial class LocalNetwork : Node, INetworkProvider
{
    public bool Exists => true;
    public int MaxSupportedPlayers => 4096;

    public event Action ActiveChanged;
    public bool Active { get; private set => this.Set(ref field, value, ActiveChanged); } = true;
}
