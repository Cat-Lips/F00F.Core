using System;
using Godot;

namespace F00F;

public static class ModelExtensions
{
    public static void Init(this Model cfg, CollisionObject3D root, Action OnPostInit)
        => cfg.Init(root, _ => cfg.PartsShape, () => cfg.BoundingShape, OnPostInit);
}
