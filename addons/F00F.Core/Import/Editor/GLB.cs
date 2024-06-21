using System;
using Godot;

namespace F00F;

public enum GlbRotate { None, Flip, Left, Right }
public enum GlbBodyType { None, RigidBody, StaticBody, VehicleBody, CharacterBody, AnimatableBody, Area }
public enum GlbShapeType { None, Convex, MultiConvex, SimpleConvex, Trimesh, Box, Sphere, Capsule, CapsuleX, CapsuleY, CapsuleZ, CylinderX, CylinderY, CylinderZ, CylinderLong, CylinderShort, RayShapeX, RayShapeY, RayShapeZ, RayCastX, RayCastY, RayCastZ, Wheel }

public static partial class GLB
{
    #region Defaults

    public static class Default
    {
        public const string RootType = nameof(GlbBodyType.RigidBody);
        public const GlbRotate Rotation = GlbRotate.None;
        public const float MassMultiplier = 1f;
        public const float ScaleMultiplier = 1f;
        public const GlbShapeType ShapeType = GlbShapeType.MultiConvex;
    }

    #endregion

    public static Node ApplyPhysics(string scene, ConfigFile cfg)
    {
        var source = LoadScene(scene);
        if (source is null) return null;

        var root = CreateRoot();
        if (root is not null)
        {
            AddParts(own: Editor.IsEditor, root, source,
                cfg.GetE(scene, "Rotation", Default.Rotation),
                cfg.GetV(scene, "MassMultiplier", Default.MassMultiplier),
                cfg.GetV(scene, "ScaleMultiplier", Default.ScaleMultiplier),
                x => cfg.GetV(scene, $"{x.Name}.Name", x.Name),
                x => cfg.GetV(scene, $"{x.Name}.ShapeType", Default.ShapeType),
                x => cfg.GetV(scene, $"{x.Name}.MultiConvexLimit", -1),
                (x, v) => cfg.SetV(scene, $"{x.Name}.MultiConvexLimit", v));
        }

        source.Free();
        return root;

        Node CreateRoot()
        {
            var rootType = cfg.GetV(scene, "RootType", Default.RootType);
            return Enum.TryParse(rootType, out GlbBodyType bodyType)
                ? CreateBody(bodyType) : LoadScene(rootType);
        }
    }
}
