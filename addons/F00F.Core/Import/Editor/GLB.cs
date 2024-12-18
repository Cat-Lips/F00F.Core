﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Godot;

namespace F00F;

public enum GlbRotate { None, Flip, Left, Right }
public enum GlbBodyType { None, Static, Rigid, Area, Character, Animatable, Vehicle }
public enum GlbShapeType { None, Convex, MultiConvex, SimpleConvex, Trimesh, Box, Sphere, Capsule, CapsuleX, CapsuleY, CapsuleZ, CylinderX, CylinderY, CylinderZ, CylinderLong, CylinderShort, RayShapeX, RayShapeY, RayShapeZ, RayCastX, RayCastY, RayCastZ, Wheel }

public static class GLB
{
    public static readonly StringName Aabb = "Aabb";
    public static readonly StringName Mass = "Mass";

    public static bool Load(string path, out Node scene, out Error err, out string msg)
    {
        Debug.Assert(path.GetExtension() is "glb" or "gltf");

        var doc = new GltfDocument();
        var state = new GltfState();

        err = doc.AppendFromFile(path, state);
        if (err is Error.Ok)
        {
            msg = null;
            scene = doc.GenerateScene(state);
            return true;
        }

        msg = $"Error loading {path.GetExtension()} [{path}]";
        scene = null;
        return false;
    }

    public static bool Save(string path, Node scene, out Error err, out string msg)
    {
        Debug.Assert(path.GetExtension() is "glb" or "gltf");

        var doc = new GltfDocument();
        var state = new GltfState();

        err = doc.AppendFromScene(scene, state);
        if (err is Error.Ok)
        {
            err = doc.WriteToFilesystem(state, path);
            if (err is Error.Ok)
            {
                msg = null;
                return true;
            }
        }

        msg = $"Error saving {path.GetExtension()} [{path}]";
        return false;
    }

    public static Node ApplyPhysics(Node source, GlbOptions options)
    {
        if (options.ImportOriginal)
            return source;

        var optIdx = 0;
        var nodeOptions = options.Nodes.ToDictionary(x => x.Name, x => (Opt: x, Idx: 0));

        CreateRoot(out var root);

        AddParts(
            own: true,
            root, source,
            options.Rotate,
            options.MassMultiplier,
            options.ScaleMultiplier,
            x => NodeOptions(x).Name,
            x => NodeOptions(x).ShapeType,
            x => NodeOptions(x).MultiConvexLimit,
            (x, v) => NodeOptions(x).MultiConvexLimit = v);

        options.Nodes = nodeOptions.Values
            .Where(x => x.Idx is not 0)
            .OrderBy(x => x.Idx)
            .Select(x => x.Opt)
            .ToArray();

        return root;

        void CreateRoot(out Node root)
        {
            root = CreateScene(options.Scene) ?? CreateBody(options.BodyType);
            root.Name = options.Name ??= source.Name;
        }

        GlbOptions.GlbNode NodeOptions(Node part)
        {
            if (!nodeOptions.TryGetValue(part.Name, out var options))
                options = (new() { Name = part.Name }, 0);

            if (options.Idx is 0)
            {
                options.Idx = ++optIdx;
                nodeOptions[options.Opt.Name] = options;
            }

            return options.Opt;
        }
    }

    public static void AddParts(
        bool own,
        Node root,
        Node source,
        GlbRotate rotate = default,
        float xMass = 1,
        float xScale = 1,
        Func<MeshInstance3D, string> GetPartName = null,
        Func<MeshInstance3D, GlbShapeType> GetShapeType = null,
        Func<MeshInstance3D, int> GetShapeCount = null,
        Action<MeshInstance3D, int> SetShapeCount = null,
        Action<MeshInstance3D, Node3D> OnPartAdded = null)
    {
        GetPartName ??= DefaultPartName;
        GetShapeType ??= DefaultShapeType;
        GetShapeCount ??= DefaultShapeCount;

        var rootMass = 0f;
        Aabb rootAabb = default;
        var offset = GetOffset(rotate, xScale);

        source?.RecurseChildren<MeshInstance3D>(self: true).ForEach(AddPart);

        SetRootMass(root, rootMass);
        SetRootAabb(root, rootAabb);

        void AddPart(MeshInstance3D part)
        {
            var partName = GetPartName(part);
            var shapeType = GetShapeType(part);
            var shapeCount = GetShapeCount(part);

            var xform = part.GlobalTransform().TransformedBy(offset);
            var shapes = CreateShapes(part.Mesh, xform, shapeType, shapeCount);
            if (shapeType is GlbShapeType.MultiConvex) SetShapeCount?.Invoke(part, shapes.Length);

            if (shapes.Length is 0 or 1) AddShape(part.Copy(), shapes.SingleOrDefault());
            else shapes.ForEach(shape => AddShape(part.Clip((CollisionShape3D)shape), shape, multi: true));

            void AddShape(MeshInstance3D mesh, Node3D shape, bool multi = false)
            {
                var name = partName ??= mesh.Name;
                if (multi) name += "1";

                if (shape is null)
                {
                    mesh.Name = name;
                    root.AddChild(mesh, own: own);
                    mesh.GlobalTransform(xform);

                    SetMeshMass();

                    OnPartAdded?.Invoke(part, mesh);
                }
                else
                {
                    shape.Name = name;
                    root.AddChild(shape, own: own);
                    shape.GlobalTransform(shape.Transform);

                    mesh.Name = "Mesh";
                    shape.AddChild(mesh, own: own);
                    mesh.GlobalTransform(xform);

                    SetMeshMass();

                    OnPartAdded?.Invoke(part, shape);
                }

                void SetMeshMass()
                {
                    var aabb = mesh.GetAabb().TransformedBy(xform);
                    var mass = aabb.Volume * xMass;
                    shape?.SetMeta(Aabb, aabb);
                    shape?.SetMeta(Mass, mass);
                    mesh.SetMeta(Aabb, aabb);
                    mesh.SetMeta(Mass, mass);
                    rootAabb = rootAabb.Merge(aabb);
                    rootMass += mass;
                }
            }
        }
    }

    #region Utils

    private static Node CreateScene(string scene)
    {
        return
            scene is "" or null ? null :
            scene.GetExtension() is "tscn" or "scn" ? GD.Load<PackedScene>(scene)?.InstantiateOrNull<Node>() :
            CreateScene(GetTscnFromType(scene));

        static string GetTscnFromType(string type)
        {
            return GetTscn(GetType(type));

            string GetTscn(Type type)
                => type?.GetCustomAttribute<ScriptPathAttribute>(false)?.Path.Replace(".cs", ".tscn");

            Type GetType(string name)
                => name.Contains('.') ? Type.GetType(name) : Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(t => t.Name == name);
        }
    }

    private static Node CreateBody(GlbBodyType body) => body switch
    {
        GlbBodyType.None => new Node(),
        GlbBodyType.Area => new Area3D(),
        GlbBodyType.Rigid => new RigidBody3D(),
        GlbBodyType.Static => new StaticBody3D(),
        GlbBodyType.Vehicle => new VehicleBody3D(),
        GlbBodyType.Character => new CharacterBody3D(),
        GlbBodyType.Animatable => new AnimatableBody3D(),
        _ => throw new NotImplementedException(),
    };

    private static Node3D[] CreateShapes(Mesh mesh, in Transform3D xform, GlbShapeType shape, int max = 0) => shape switch
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

    private static Transform3D GetOffset(GlbRotate rotate, float scale)
    {
        return Transform3D.Identity
            .Rotated(Vector3.Up, Angle())
            .Scaled(Vector3.One * scale);

        float Angle() => rotate switch
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

        [Conditional("DEBUG")]
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

        [Conditional("DEBUG")]
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

    internal static string DefaultPartName(Node part) => part.Name;
    internal static GlbShapeType DefaultShapeType(Node part) => GlbShapeType.SimpleConvex;
    internal static int DefaultShapeCount(Node part) => 0;

    internal static void RemoveMeta(Node root)
    {
        if (root is null) return;
        if (root is RigidBody3D body) body.Mass = 1;
        else root.RemoveMeta(Mass);
        root.RemoveMeta(Aabb);
    }

    #endregion
}
