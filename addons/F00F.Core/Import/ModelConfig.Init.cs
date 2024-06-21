using System;
using Godot;

namespace F00F;

public static class ModelConfigExtensions
{
    public static void Init(
        this ModelConfig cfg,
        CollisionObject3D root,
        Func<MeshInstance3D, GlbShapeType> GetShapeType,
        Func<GlbSimpleShapeType> GetBoundingShape = null,
        Action OnPostInit = null)
        => cfg.Init(root, null, OnPostInit, GetShapeType, null, null, null, GetBoundingShape);

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
        root.OnReady(Init);

        void Init()
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
                    GLB.InitPhysics(root, cfg.Source.GetNode(root), cfg.Scene,
                        cfg.Rotation, cfg.MassMultiplier, cfg.ScaleMultiplier, cfg.ShapeReductionRatio,
                        GetShapeType, GetBoundingShape, GetShapeCount, SetShapeCount, OnPartAdded);
                }
            }
        }
    }
}
