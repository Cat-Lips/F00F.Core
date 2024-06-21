using System;
using Godot;

namespace F00F;

public interface IWorld
{
    event Action<Vector3> SpawnReady;
    Vector3? SpawnPoint { get; }
}
