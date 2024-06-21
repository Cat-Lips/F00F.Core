using System;
using Godot;

namespace F00F;

public static class PhysicsConfigExtensions
{
    public static void Init(
        this PhysicsConfig cfg,
        CollisionObject3D root,
        Action OnPreInit = null,
        Action OnPostInit = null,
        Func<MeshInstance3D, GlbShapeType> GetShapeType = null,
        Func<MeshInstance3D, int> GetShapeCount = null,
        Action<MeshInstance3D, int> SetShapeCount = null,
        Action<Node3D> OnPartAdded = null)
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
                    GetShapeType, GetShapeCount, SetShapeCount, OnPartAdded);
            }
        }
    }
}
