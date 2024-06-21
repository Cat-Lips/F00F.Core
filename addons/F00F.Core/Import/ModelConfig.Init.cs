using System;
using Godot;

namespace F00F;

public static class ModelConfigExtensions
{
    public static void Init(
        this ModelConfig cfg,
        CollisionObject3D root,
        Func<MeshInstance3D, GlbShapeType> GetShapeType = null,
        Func<GlbSimpleShapeType> GetBoundingShape = null)
        => cfg.Init(root, null, null, GetShapeType, null, null, null, GetBoundingShape);

    public static void Init(
        this ModelConfig cfg,
        CollisionObject3D root,
        Action OnPreInit = null,
        Action OnPostInit = null,
        Func<MeshInstance3D, GlbShapeType> GetShapeType = null,
        Func<MeshInstance3D, int> GetShapeCount = null,
        Action<MeshInstance3D, int> SetShapeCount = null,
        Action<Node3D> OnPartAdded = null,
        Func<GlbSimpleShapeType> GetBoundingShape = null)
    {
        if (!cfg.SafeInit(root, InitPhysics))
            GLB.PurgeParts(root);

        void InitPhysics()
        {
            OnPreInit?.Invoke();
            InitPhysics();
            OnPostInit?.Invoke();

            void InitPhysics()
            {
                GLB.InitPhysics(root, cfg.Scene,
                    cfg.Rotation, cfg.MassMultiplier, cfg.ScaleMultiplier,
                    GetShapeType, GetShapeCount, SetShapeCount, OnPartAdded,
                    GetBoundingShape);
            }
        }
    }
}
