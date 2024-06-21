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
        PackedScene scene = null,
        GlbRotate rotation = Default.Rotation,
        float massMultiplier = Default.MassMultiplier,
        float scaleMultiplier = Default.ScaleMultiplier,
        Func<MeshInstance3D, GlbShapeType> GetShapeType = null,
        Func<MeshInstance3D, int> GetShapeCount = null,
        Action<MeshInstance3D, int> SetShapeCount = null,
        Action<Node3D> OnPartAdded = null,
        Func<GlbSimpleShapeType> GetBoundingShape = null)
    {
        PurgeParts(root);

        var model = scene?.Instantiate();
        if (!root.IsEditedSceneRoot(includeInherited: true))
            root.Name = Default.NodeName(model?.Name ?? root.GetType().Name);

        var gform = root.GlobalTransform();
        root.GlobalTransform(Transform3D.Identity);

        AddParts(root, model, rotation, massMultiplier, scaleMultiplier, null,
            GetShapeType, GetShapeCount, SetShapeCount, _OnPartAdded, GetBoundingShape);

        root.GlobalTransform(gform);
        if (root is RigidBody3D body)
            body.Freeze = model is null;
        model?.Free();

        void _OnPartAdded(Node3D part)
        {
            part.SetMeta(IsGenerated, true);
            OnPartAdded?.Invoke(part);
        }
    }

    public static void AddPart(Node part)
        => part.SetMeta(IsGenerated, true);

    public static bool IsPart(Node part)
        => part.HasMeta(IsGenerated);

    public static void PurgeParts(CollisionObject3D root)
        => root.RemoveChildren(IsPart);

    [Conditional("TOOLS")]
    public static void OnEditorSave(CollisionObject3D root)
    {
        if (root is RigidBody3D body)
        {
            Editor.DoPreSaveReset(body, RigidBody3D.PropertyName.Mass, 1);
            Editor.DoPreSaveReset(body, RigidBody3D.PropertyName.Freeze);
        }

        Editor.DoPreSaveResetMeta(root, GLB.Aabb, GLB.Mass);
        root.ForEachChild<Node>(IsPart, x => Editor.DoPreSaveReset(x, Node.PropertyName.Owner));
    }
}
