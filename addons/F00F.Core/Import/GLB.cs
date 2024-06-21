using System;
using System.Diagnostics;
using Godot;

namespace F00F;

public enum GlbRotate { None, Flip, Left, Right }
public enum GlbBodyType { None, RigidBody, StaticBody, VehicleBody, CharacterBody, AnimatableBody, Area }
public enum GlbShapeType { None, Convex, MultiConvex, SimpleConvex, Trimesh, Box, Sphere, Capsule, CapsuleX, CapsuleY, CapsuleZ, CylinderX, CylinderY, CylinderZ, CylinderLong, CylinderShort, RayShapeX, RayShapeY, RayShapeZ, RayCastX, RayCastY, RayCastZ, Wheel }
public enum GlbSimpleShapeType { None, Box, Sphere, Capsule, CylinderLong, CylinderShort }

public static partial class GLB
{
    private static readonly StringName IsGenerated = "_GLB_";

    #region Defaults

    public static class Default
    {
        public const string RootType = nameof(GlbBodyType.RigidBody);
        public const GlbRotate Rotation = GlbRotate.Flip;
        public const float MassMultiplier = 1f;
        public const float MeshScaleMultiplier = 1f;
        public const float ShapeScaleMultiplier = 1f;
        public const int MultiConvexShapeLimit = -1;
        public const GlbShapeType ShapeType = GlbShapeType.Convex;

        public static string NodeName(StringName source) => source.ToSafeCapitalCase();

        public static string GetPartName(MeshInstance3D source) => NodeName(source.Name);
        public static int GetShapeCount(MeshInstance3D _) => MultiConvexShapeLimit;
        public static GlbShapeType GetPartShape(MeshInstance3D _) => ShapeType;
    }

    #endregion

    public static void InitPhysics(
        CollisionObject3D root,
        Node source = null,
        PackedScene scene = null,
        GlbRotate rotation = Default.Rotation,
        float massMultiplier = Default.MassMultiplier,
        float meshScaleMultiplier = Default.MeshScaleMultiplier,
        float shapeScaleMultiplier = Default.ShapeScaleMultiplier,
        Action<Node3D> OnPartAdded = null,
        Func<MeshInstance3D, int> GetShapeCount = null,
        Action<MeshInstance3D, int> SetShapeCount = null,
        Func<MeshInstance3D, GlbShapeType> GetPartShape = null,
        Func<GlbSimpleShapeType> GetBoundingShape = null)
    {
        PurgeParts(root);

        Debug.Assert(source is null || scene is null);
        var mode = scene is null ? MeshMode.Static : MeshMode.Dynamic;

        var model = source ?? scene?.Instantiate();
        if (mode is MeshMode.Dynamic && !root.IsEditedSceneRoot(includeInherited: true))
            root.Name = Default.NodeName(model?.Name ?? root.GetType().Name);

        var gform = root.GlobalTransform();
        root.GlobalTransform(Transform3D.Identity);

        AddParts(root, owner: null, model, mode, rotation, massMultiplier, meshScaleMultiplier, shapeScaleMultiplier,
            _OnPartAdded, GetShapeCount, SetShapeCount, GetPartName: null, GetPartShape, GetBoundingShape);

        root.GlobalTransform(gform);
        if (root is RigidBody3D body)
            body.Freeze = model is null;
        if (source is null)
            model?.Free();

        void _OnPartAdded(Node3D part)
        {
            AddPart(part);
            OnPartAdded?.Invoke(part);
        }
    }

    public static void AddPart(Node part)
        => part.SetMeta(IsGenerated, true);

    public static bool IsPart(Node part)
        => part.HasMeta(IsGenerated);

    public static void PurgeParts(CollisionObject3D root)
        => root.RemoveChildren(IsPart);

    public static void CopyParts(CollisionObject3D root, CollisionObject3D to)
        => root.ForEachChild(IsPart, part => to.AddChild(part.Copy(), owner: root));

    public static MeshInstance3D GetMesh(CollisionShape3D shape)
        => (MeshInstance3D)shape.GetNode((NodePath)shape.GetMeta(Mesh));
}
