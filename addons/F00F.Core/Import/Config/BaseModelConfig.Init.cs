using System;
using Godot;

namespace F00F;

public static class BaseModelConfigExtensions
{
    public static void Init(
        this BaseModelConfig cfg,
        CollisionObject3D root,
        Action OnPreInit,
        Action OnPostInit,
        Action<Node3D> OnPartAdded,
        Func<MeshInstance3D, int> GetShapeCount,
        Action<MeshInstance3D, int> SetShapeCount,
        Func<MeshInstance3D, GlbShapeType> GetShape)
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
                    GLB.InitPhysics(root, null,
                        cfg.Scene, cfg.Rotation, cfg.MassMultiplier,
                        cfg.MeshScaleMultiplier, cfg.ShapeScaleMultiplier,
                        OnPartAdded, GetShapeCount, SetShapeCount, GetShape);
                }
            }
        }
    }
}
