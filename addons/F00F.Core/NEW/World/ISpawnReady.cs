using System;
using Godot;

namespace F00F;

public interface ISpawnReady
{
    event Action<Vector3> SpawnReady;
    Vector3? SpawnPoint { get; }
}
