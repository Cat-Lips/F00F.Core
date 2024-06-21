using System;
using Godot;

namespace F00F;

public static class ModelConfigExtensions
{
    public static void Init(this ModelConfig cfg, CollisionObject3D root, Action OnPostInit = null)
    {
        cfg.Init(
            root,
            OnPreInit: null,
            OnPostInit,
            OnPartAdded: null,
            GetShapeCount: null,
            SetShapeCount: null,
            GetPartsShape: _ => cfg.PartsShape,
            GetBoundingShape: () => cfg.BoundingShape);
    }

    public static void Init(
        this ModelConfig cfg,
        CollisionObject3D root,
        Action OnPreInit,
        Action OnPostInit,
        Action<Node3D> OnPartAdded,
        Func<MeshInstance3D, int> GetShapeCount,
        Action<MeshInstance3D, int> SetShapeCount,
        Func<MeshInstance3D, GlbShapeType> GetPartsShape,
        Func<GlbSimpleShapeType> GetBoundingShape)
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
                    GLB.InitPhysics(root, cfg.Source.GetNode(root),
                        cfg.Scene, cfg.Rotation, cfg.MassMultiplier,
                        cfg.MeshScaleMultiplier, cfg.ShapeScaleMultiplier,
                        OnPartAdded, GetShapeCount, SetShapeCount, GetPartsShape, GetBoundingShape);
                }
            }
        }
    }
}
