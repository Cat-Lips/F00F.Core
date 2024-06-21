#if DEBUG
//#define CHECK
#endif

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Godot;

namespace F00F;

public static partial class GLB
{
    public static readonly StringName Aabb = "Aabb";
    public static readonly StringName Mass = "Mass";
    public static readonly StringName Mesh = "Mesh";

    private static Node LoadScene(string scene)
    {
        return
            scene is "" or null ? null :
            scene.GetExtension() is "tscn" or "scn" ? GD.Load<PackedScene>(scene)?.InstantiateOrNull<Node>() :
            LoadScene(GetTscnFromType(scene));

        static string GetTscnFromType(string name)
        {
            var type = name.Contains('.') ? Type.GetType(name) :
                Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(t => t.Name == name);
            return type?.GetCustomAttribute<ScriptPathAttribute>(false)?.Path.Replace(".cs", ".tscn");
        }
    }

    private static Node CreateBody(GlbBodyType body) => body switch
    {
        GlbBodyType.None => new Node(),
        GlbBodyType.RigidBody => new RigidBody3D(),
        GlbBodyType.StaticBody => new StaticBody3D(),
        GlbBodyType.VehicleBody => new VehicleBody3D(),
        GlbBodyType.CharacterBody => new CharacterBody3D(),
        GlbBodyType.AnimatableBody => new AnimatableBody3D(),
        GlbBodyType.Area => new Area3D(),
        _ => throw new NotImplementedException(),
    };

    private static Node3D[] CreateShapes(Mesh mesh, in Transform3D xform, GlbShapeType shape, int max = -1) => shape switch
    {
        GlbShapeType.None => [],
        GlbShapeType.Convex => [mesh.CreateShape(xform)],
        GlbShapeType.MultiConvex => mesh.CreateShapes(xform, max),
        GlbShapeType.SimpleConvex => [mesh.CreateShape(xform, simplify: true)],
        GlbShapeType.Trimesh => [mesh.CreateShapeAsTrimesh(xform)],
        GlbShapeType.Box => [mesh.CreateBoxShape(xform)],
        GlbShapeType.Sphere => [mesh.CreateSphereShape(xform)],
        GlbShapeType.Capsule => [mesh.CreateCapsuleShape(xform)],
        GlbShapeType.CapsuleX => [mesh.CreateCapsuleShape(xform, Vector3.Axis.X)],
        GlbShapeType.CapsuleY => [mesh.CreateCapsuleShape(xform, Vector3.Axis.Y)],
        GlbShapeType.CapsuleZ => [mesh.CreateCapsuleShape(xform, Vector3.Axis.Z)],
        GlbShapeType.CylinderX => [mesh.CreateCylinderShape(xform, Vector3.Axis.X)],
        GlbShapeType.CylinderY => [mesh.CreateCylinderShape(xform, Vector3.Axis.Y)],
        GlbShapeType.CylinderZ => [mesh.CreateCylinderShape(xform, Vector3.Axis.Z)],
        GlbShapeType.CylinderLong => [mesh.CreateCylinderShapeLong(xform)],
        GlbShapeType.CylinderShort => [mesh.CreateCylinderShapeShort(xform)],
        GlbShapeType.RayShapeX => [mesh.CreateRayShape(xform, Vector3.Axis.X, slide: true)],
        GlbShapeType.RayShapeY => [mesh.CreateRayShape(xform, Vector3.Axis.Y, slide: true)],
        GlbShapeType.RayShapeZ => [mesh.CreateRayShape(xform, Vector3.Axis.Z, slide: true)],
        GlbShapeType.RayCastX => [mesh.CreateRayCast(xform, Vector3.Axis.X)],
        GlbShapeType.RayCastY => [mesh.CreateRayCast(xform, Vector3.Axis.Y)],
        GlbShapeType.RayCastZ => [mesh.CreateRayCast(xform, Vector3.Axis.Z)],
        GlbShapeType.Wheel => [mesh.CreateWheelShape(xform)],
        _ => throw new NotImplementedException(),
    };

    private static CollisionShape3D SimpleShape(in Aabb bb, GlbSimpleShapeType shape) => shape switch
    {
        GlbSimpleShapeType.None => null,
        GlbSimpleShapeType.Box => bb.CreateBoxShape(),
        GlbSimpleShapeType.Sphere => bb.CreateSphereShape(),
        GlbSimpleShapeType.Capsule => bb.CreateCapsuleShape(),
        GlbSimpleShapeType.CylinderLong => bb.CreateCylinderShapeLong(),
        GlbSimpleShapeType.CylinderShort => bb.CreateCylinderShapeShort(),
        _ => throw new NotImplementedException(),
    };

    private static Transform3D GetOffset(GlbRotate rotation, float scale)
    {
        return Transform3D.Identity
            .Rotated(Vector3.Up, Angle())
            .Scaled(Vector3.One * scale);

        float Angle() => rotation switch
        {
            GlbRotate.None => 0,
            GlbRotate.Flip => Const.Deg180,
            GlbRotate.Left => Const.Deg90,
            GlbRotate.Right => -Const.Deg90,
            _ => throw new NotImplementedException(),
        };
    }

    private static void SetRootMass(Node root, float mass)
    {
        if (root is RigidBody3D body) body.Mass = mass is 0 ? Const.Epsilon : mass;
        else root.SetMeta(Mass, mass);
        Check(root, mass);

        [Conditional("CHECK")]
        static void Check(Node root, float value)
        {
            var mesh = Mesh().IfAny(_ => _.Sum(x => (float)x.GetMeta(Mass)));
            var shape = Shape().IfAny(_ => _.Sum(x => (float)x.GetMeta(Mass)));
            var source = Mesh().IfAny(_ => _.Sum(x => x.GetAabb().TransformedBy(x.GlobalTransform()).Volume));
            if (Fail().Any()) GD.Print($"Expected Mass = {value} [Name: {root.Name}, {string.Join(", ", Fail())}]");

            IEnumerable<MeshInstance3D> Mesh()
            {
                return root.RecurseChildren<MeshInstance3D>()
                    .Where(x => x.HasMeta(Mass));
            }

            IEnumerable<Node3D> Shape()
            {
                return root.RecurseChildren<CollisionShape3D>().Cast<Node3D>()
                    .Concat(root.RecurseChildren<VehicleWheel3D>())
                    .Concat(root.RecurseChildren<RayCast3D>())
                    .Where(x => x.HasMeta(Mass));
            }

            IEnumerable<string> Fail()
            {
                if (mesh is not 0 && !Mathf.IsEqualApprox(mesh, value)) yield return $"Mesh: {mesh}";
                if (shape is not 0 && !Mathf.IsEqualApprox(shape, value)) yield return $"Shape: {shape}";
                if (source is not 0 && !Mathf.IsEqualApprox(source, value)) yield return $"Source: {source}";
            }
        }
    }

    private static void SetRootAabb(Node root, in Aabb aabb)
    {
        root.SetMeta(Aabb, aabb);
        Check(root, aabb);

        [Conditional("CHECK")]
        static void Check(Node root, Aabb value)
        {
            var mesh = Mesh().IfAny(_ => _.Select(x => (Aabb)x.GetMeta(Aabb)).Aggregate((a, b) => a.Merge(b)));
            var shape = Shape().IfAny(_ => _.Select(x => (Aabb)x.GetMeta(Aabb)).Aggregate((a, b) => a.Merge(b)));
            var source = Mesh().IfAny(_ => _.Select(x => x.GetAabb().TransformedBy(x.GlobalTransform())).Aggregate((a, b) => a.Merge(b)));
            if (Fail().Any()) GD.Print($"Expected Aabb = {value} [Name: {root.Name}, {string.Join(", ", Fail())}]");

            IEnumerable<MeshInstance3D> Mesh()
            {
                return root.RecurseChildren<MeshInstance3D>()
                    .Where(x => x.HasMeta(Aabb));
            }

            IEnumerable<Node3D> Shape()
            {
                return root.RecurseChildren<CollisionShape3D>().Cast<Node3D>()
                    .Concat(root.RecurseChildren<VehicleWheel3D>())
                    .Concat(root.RecurseChildren<RayCast3D>())
                    .Where(x => x.HasMeta(Aabb));
            }

            IEnumerable<string> Fail()
            {
                if (mesh.HasVolume() && !mesh.IsEqualApprox(value)) yield return $"Mesh: {mesh}";
                if (shape.HasVolume() && !shape.IsEqualApprox(value)) yield return $"Shape: {shape}";
                if (source.HasVolume() && !source.IsEqualApprox(value)) yield return $"Source: {source}";
            }
        }
    }
}
