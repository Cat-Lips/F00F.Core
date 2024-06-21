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
        public const float ScaleMultiplier = 1f;
        public const float ShapeReductionRatio = 0f;
        public const GlbShapeType ShapeType = GlbShapeType.Convex;
        public const int MultiConvexShapeLimit = -1;

        public static string NodeName(StringName source) => source.ToSafeCapitalCase();

        public static string GetRootName(Node source) => NodeName(source.Name);
        public static GlbRotate GetRotation(Node _) => Rotation;
        public static float GetMassMultiplier(Node _) => MassMultiplier;
        public static float GetScaleMultiplier(Node _) => ScaleMultiplier;
        public static string GetPartName(MeshInstance3D source) => NodeName(source.Name);
        public static GlbShapeType GetShapeType(MeshInstance3D _) => ShapeType;
        public static int GetShapeCount(MeshInstance3D _) => MultiConvexShapeLimit;
    }

    #endregion

    public static void InitPhysics(
        CollisionObject3D root,
        Node source = null,
        PackedScene scene = null,
        GlbRotate rotation = Default.Rotation,
        float massMultiplier = Default.MassMultiplier,
        float scaleMultiplier = Default.ScaleMultiplier,
        float shapeReductionRatio = Default.ShapeReductionRatio,
        Func<MeshInstance3D, GlbShapeType> GetShapeType = null,
        Func<GlbSimpleShapeType> GetBoundingShape = null,
        Func<MeshInstance3D, int> GetShapeCount = null,
        Action<MeshInstance3D, int> SetShapeCount = null,
        Action<Node3D> OnPartAdded = null)
    {
        PurgeParts(root);

        Debug.Assert(source is null || scene is null);
        var mode = scene is null ? MeshMode.Static : MeshMode.Dynamic;

        var model = source ?? scene?.Instantiate();
        if (mode is MeshMode.Dynamic && !root.IsEditedSceneRoot(includeInherited: true))
            root.Name = Default.NodeName(model?.Name ?? root.GetType().Name);

        var gform = root.GlobalTransform();
        root.GlobalTransform(Transform3D.Identity);

        AddParts(root, null, model, mode, rotation, massMultiplier, scaleMultiplier, shapeReductionRatio,
            null, GetShapeType, GetBoundingShape, GetShapeCount, SetShapeCount, _OnPartAdded);

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
        => root.RemoveChildren(IsPart/*, recurse: true*/);

    public static MeshInstance3D GetMesh(CollisionShape3D shape)
        => shape.GetNode<MeshInstance3D>((NodePath)shape.GetMeta(Mesh));

    [Conditional("TOOLS")]
    public static void OnEditorSave(CollisionObject3D root)
    {
        if (root is RigidBody3D body)
        {
            Editor.DoPreSaveReset(body, RigidBody3D.PropertyName.Mass, 1);
            Editor.DoPreSaveReset(body, RigidBody3D.PropertyName.Freeze);
        }

        Editor.DoPreSaveResetMeta(root, GLB.Aabb, GLB.Mass);
        Editor.DoPreSaveResetOwner(root, where: IsPart);
    }
}
