using Godot;

namespace F00F;

public static class PlayerModelExtensions
{
    public static void Init(this PlayerModel cfg, CollisionObject3D root)
        => cfg.Init(root, _ => cfg.PartsShape, () => cfg.BoundingShape);
}
