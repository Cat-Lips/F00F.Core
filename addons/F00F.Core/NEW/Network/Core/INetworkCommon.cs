using System;
using Godot;

namespace F00F.NEW;

internal interface INetworkCommon
{
    event Action ActiveChanged;
    StringName Name { get; }
    bool Exists => false;
    bool Active => false;
}
