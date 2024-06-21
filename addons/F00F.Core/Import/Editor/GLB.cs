using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Godot;

namespace F00F
{
    public enum GlbFrontFace { Z, X, NegZ, NegX }
    public enum GlbBodyType { None, Static, Rigid, Area, Character, Animatable, Vehicle }
    public enum GlbShapeType { None, Convex, MultiConvex, SimpleConvex, Trimesh, Box, Sphere, CapsuleX, CapsuleY, CapsuleZ, CylinderX, CylinderY, CylinderZ, RayShapeX, RayShapeY, RayShapeZ, RayCastX, RayCastY, RayCastZ, Wheel }

    public static class GLB
    {
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
                options.FrontFace,
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

        public static void AddParts(bool own, Node root, Node source, GlbFrontFace front = GlbFrontFace.NegZ, float xMass = 1, float xScale = 1, Func<MeshInstance3D, string> GetPartName = null, Func<MeshInstance3D, GlbShapeType> GetShapeType = null, Func<MeshInstance3D, int> GetShapeCount = null, Action<MeshInstance3D, int> SetShapeCount = null, Action<MeshInstance3D, Node3D> OnPartAdded = null)
        {
            if (xMass is 0) xMass = 1;
            if (xScale is 0) xScale = 1;

            GetPartName ??= DefaultPartName;
            GetShapeType ??= DefaultShapeType;
            GetShapeCount ??= DefaultShapeCount;

            var rootMass = 1f;
            Aabb rootAabb = default;
            var offset = GetRootOffset(root, front, xScale);

            source?.RecurseChildren<MeshInstance3D>(self: true).ForEach(AddPart);

            SetRootMass(root, rootMass);
            SetRootAabb(root, rootAabb);

            void AddPart(MeshInstance3D part)
            {
                var partName = GetPartName(part);
                var shapeType = GetShapeType(part);
                var shapeCount = GetShapeCount(part);

                var xform = part.GlobalTransform();
                var shapes = CreateShapes(part.Mesh, shapeType, shapeCount);
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
                        mesh.GlobalTransform(offset * xform);

                        SetMeshMass();

                        OnPartAdded?.Invoke(part, mesh);
                    }
                    else
                    {
                        shape.Name = name;
                        root.AddChild(shape, own: own);
                        shape.GlobalTransform(offset * xform * shape.Transform);

                        mesh.Name = "Mesh";
                        mesh.Visible = false;
                        shape.AddChild(mesh, own: own);
                        mesh.GlobalTransform(offset * xform);

                        SetMeshMass();

                        OnPartAdded?.Invoke(part, shape);
                    }

                    void SetMeshMass()
                    {
                        var aabb = mesh.GlobalTransform() * mesh.GetAabb();
                        var mass = aabb.Volume * xMass;
                        shape?.SetMeta("Aabb", aabb);
                        shape?.SetMeta("Mass", mass);
                        mesh.SetMeta("Aabb", aabb);
                        mesh.SetMeta("Mass", mass);
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

        private static Node3D[] CreateShapes(Mesh mesh, GlbShapeType shape, int max = 0) => shape switch
        {
            GlbShapeType.None => [],
            GlbShapeType.Convex => [mesh.CreateShape()],
            GlbShapeType.MultiConvex => mesh.CreateShapes(max),
            GlbShapeType.SimpleConvex => [mesh.CreateShape(simplify: true)],
            GlbShapeType.Trimesh => [mesh.CreateShapeAsTrimesh()],
            GlbShapeType.Box => [mesh.CreateBoxShape()],
            GlbShapeType.Sphere => [mesh.CreateSphereShape()],
            GlbShapeType.CapsuleX => [mesh.CreateCapsuleShape(Vector3.Axis.X)],
            GlbShapeType.CapsuleY => [mesh.CreateCapsuleShape(Vector3.Axis.Y)],
            GlbShapeType.CapsuleZ => [mesh.CreateCapsuleShape(Vector3.Axis.Z)],
            GlbShapeType.CylinderX => [mesh.CreateCylinderShape(Vector3.Axis.X)],
            GlbShapeType.CylinderY => [mesh.CreateCylinderShape(Vector3.Axis.Y)],
            GlbShapeType.CylinderZ => [mesh.CreateCylinderShape(Vector3.Axis.Z)],
            GlbShapeType.RayShapeX => [mesh.CreateRayShape(Vector3.Axis.X, slide: true)],
            GlbShapeType.RayShapeY => [mesh.CreateRayShape(Vector3.Axis.Y, slide: true)],
            GlbShapeType.RayShapeZ => [mesh.CreateRayShape(Vector3.Axis.Z, slide: true)],
            GlbShapeType.RayCastX => [mesh.CreateRayCast(Vector3.Axis.X)],
            GlbShapeType.RayCastY => [mesh.CreateRayCast(Vector3.Axis.Y)],
            GlbShapeType.RayCastZ => [mesh.CreateRayCast(Vector3.Axis.Z)],
            GlbShapeType.Wheel => [mesh.CreateWheelShape()],
            _ => throw new NotImplementedException(),
        };

        private static Transform3D GetRootOffset(Node root, GlbFrontFace front, float scale)
        {
            return Root() * Offset();

            Transform3D Root()
                => (root as Node3D)?.GlobalTransform() ?? Transform3D.Identity;

            Transform3D Offset()
            {
                return Transform3D.Identity
                    .Rotated(Vector3.Up, Angle())
                    .Scaled(Vector3.One * scale);

                float Angle() => front switch
                {
                    GlbFrontFace.Z => 0,
                    GlbFrontFace.X => Const.Deg90,
                    GlbFrontFace.NegZ => Const.Deg180,
                    GlbFrontFace.NegX => -Const.Deg90,
                    _ => throw new NotImplementedException(),
                };
            }
        }

        private static void SetRootMass(Node root, float mass)
        {
            if (root is RigidBody3D body) body.Mass = mass;
            else root.SetMeta("Mass", mass);
        }

        private static void SetRootAabb(Node root, in Aabb aabb)
            => root.SetMeta("Aabb", aabb);

        private static string DefaultPartName(Node part) => part.Name;
        private static GlbShapeType DefaultShapeType(Node part) => GlbShapeType.MultiConvex;
        private static int DefaultShapeCount(Node part) => 0;

        #endregion
    }
}
