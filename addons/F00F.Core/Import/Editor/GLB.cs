using System;
using System.Diagnostics;
using System.Linq;
using Godot;

namespace F00F
{
    public enum GlbFrontFace { Z, X, NegZ, NegX }
    public enum GlbBodyType { None, Static, Rigid, Area, Character, Animatable, Vehicle }
    public enum GlbShapeType { None, Convex, MultiConvex, SimpleConvex, Trimesh, Box, Sphere, Capsule, CapsuleX, CapsuleZ, Cylinder, CylinderX, CylinderZ, Wheel }

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

            var rootMass = 1f;
            var offset = GetTransform(options.FrontFace, options.ScaleMultiplier);
            var xMass = options.MassMultiplier;

            CreateRoot(out var root);
            AddParts();
            SetRootMass(root, rootMass);

            return root;

            void CreateRoot(out Node root)
            {
                root = CreateScene(options.Scene) ?? CreateBody(options.BodyType);
                root.Name = options.Name ??= source.Name;
            }

            void AddParts()
            {
                var nodeOptions = options.Nodes.ToList();
                RecurseParts();
                options.Nodes = nodeOptions.ToArray();

                void RecurseParts()
                {
                    var partIndex = -1;
                    foreach (var node in source.RecurseChildren(self: true))
                    {
                        if (node is MeshInstance3D mesh)
                            AddShapes(mesh, nodeOptions.GetOrAdd(++partIndex));
                    }

                    void AddShapes(MeshInstance3D source, GlbOptions.GlbNode options)
                    {
                        var sourceTransform = source.GlobalTransform();
                        var shapes = CreateShapes(source.Mesh, options.ShapeType, options.MultiConvexLimit);
                        if (options.ShapeType is GlbShapeType.MultiConvex) options.MultiConvexLimit = shapes.Length;
                        if (shapes.Length is 0 or 1) AddShape(source.Copy(), shapes.SingleOrDefault());
                        else shapes.ForEach(shape => AddShape(source.Clip((CollisionShape3D)shape), shape, multi: true));

                        void AddShape(MeshInstance3D mesh, Node3D shape, bool multi = false)
                        {
                            var name = options.Name ??= mesh.Name;
                            if (multi) name += "1";

                            if (shape is null)
                            {
                                mesh.Name = name;
                                root.OwnChild(mesh);
                                mesh.GlobalTransform(offset * sourceTransform);

                                var mass = GetMeshMass(mesh) * xMass;
                                mesh.SetMeta("Mass", mass);
                                rootMass += mass;
                            }
                            else
                            {
                                shape.Name = name;
                                root.OwnChild(shape);
                                shape.GlobalTransform(offset * sourceTransform * shape.Transform);

                                mesh.Name = "Mesh";
                                shape.OwnChild(mesh);
                                mesh.GlobalTransform(offset * sourceTransform);

                                var mass = GetMeshMass(mesh) * xMass;
                                shape.SetMeta("Mass", mass);
                                mesh.SetMeta("Mass", mass);
                                rootMass += mass;
                            }
                        }
                    }
                }
            }

            #region Utils

            static Node CreateScene(PackedScene tscn)
                => tscn?.InstantiateOrNull<Node>();

            static Node CreateBody(GlbBodyType body) => body switch
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

            static Node3D[] CreateShapes(Mesh mesh, GlbShapeType shape, int max = 0) => shape switch
            {
                GlbShapeType.None => [],
                GlbShapeType.Convex => [new CollisionShape3D() { Shape = mesh.CreateConvexShape() }],
                GlbShapeType.MultiConvex => mesh.CreateShapes(max),
                GlbShapeType.SimpleConvex => [new CollisionShape3D() { Shape = mesh.CreateConvexShape(simplify: true) }],
                GlbShapeType.Trimesh => [new CollisionShape3D() { Shape = mesh.CreateTrimeshShape() }],
                GlbShapeType.Box => [mesh.CreateBoxShape()],
                GlbShapeType.Sphere => [mesh.CreateSphereShape()],
                GlbShapeType.Capsule => [mesh.CreateCapsuleShape()],
                GlbShapeType.CapsuleX => [mesh.CreateCapsuleShape(Vector3.Axis.X)],
                GlbShapeType.CapsuleZ => [mesh.CreateCapsuleShape(Vector3.Axis.Z)],
                GlbShapeType.Cylinder => [mesh.CreateCylinderShape()],
                GlbShapeType.CylinderX => [mesh.CreateCylinderShape(Vector3.Axis.X)],
                GlbShapeType.CylinderZ => [mesh.CreateCylinderShape(Vector3.Axis.Z)],
                GlbShapeType.Wheel => [mesh.CreateWheelShape()],
                _ => throw new NotImplementedException(),
            };

            static Transform3D GetTransform(GlbFrontFace frontFace, float scale)
            {
                return Transform3D.Identity.Rotated(Vector3.Up, Angle()).Scaled(Vector3.One * scale);

                float Angle() => frontFace switch
                {
                    GlbFrontFace.Z => 0,
                    GlbFrontFace.X => Const.Deg90,
                    GlbFrontFace.NegZ => Const.Deg180,
                    GlbFrontFace.NegX => -Const.Deg90,
                    _ => throw new NotImplementedException(),
                };
            }

            static float GetMeshMass(MeshInstance3D mesh)
                => (mesh.GetAabb() * mesh.GlobalTransform()).Volume;

            static void SetRootMass(Node root, float mass)
            {
                if (root is RigidBody3D body) body.Mass = mass;
                else root.SetMeta("Mass", mass);
            }

            #endregion
        }
    }
}
