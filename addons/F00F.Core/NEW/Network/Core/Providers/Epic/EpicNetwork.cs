using System;
using Godot;

namespace F00F.NEW;

[Tool]
internal partial class EpicNetwork : Node, INetworkProvider
{
    public event Action ActiveChanged;
    public bool Active { get; private set => this.Set(ref field, value, ActiveChanged); }
}
