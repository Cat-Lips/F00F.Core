using System;
using System.Linq;
using Godot;

namespace F00F;

public static partial class GLB
{
    private enum MeshMode
    {
        Static,  // Collision nodes will be linked to mesh nodes using meta-data
        Dynamic, // Mesh nodes will be copied and placed as child of collision nodes
    }

    private static void AddParts(
        Node root,
        Node owner,
        Node source,
        MeshMode mode,
        GlbRotate rotation,
        float massMultiplier,
        float meshScaleMultiplier,
        float shapeScaleMultiplier,
        Action<Node3D> OnPartAdded,
        Func<MeshInstance3D, int> GetShapeCount,
        Action<MeshInstance3D, int> SetShapeCount,
        Func<MeshInstance3D, string> GetPartName,
        Func<MeshInstance3D, GlbShapeType> GetPartShape,
        Func<GlbSimpleShapeType> GetBoundingShape)
    {
        owner ??= root;

        GetPartName ??= Default.GetPartName;
        GetPartShape ??= Default.GetPartShape;
        GetShapeCount ??= Default.GetShapeCount;
        if (mode is MeshMode.Static) rotation = GlbRotate.None; // Rotate model externally and refresh manually
        var boundingShape = GetBoundingShape?.Invoke() ?? GlbSimpleShapeType.None;
        var disableShapes = boundingShape is not GlbSimpleShapeType.None;

        float rootMass = default;
        Aabb? rootAabb = default;
        var offset = GetOffset(rotation, meshScaleMultiplier);

        source?.RecurseChildren<MeshInstance3D>(self: true).ForEach(AddPart);
        if (boundingShape is not GlbSimpleShapeType.None) AddBoundingShape();

        SetRootMass(root, rootMass);
        SetRootAabb(root, rootAabb ?? default);

        void AddPart(MeshInstance3D part)
        {
            var partName = GetPartName(part);
            var shapeType = GetPartShape(part);
            var shapeCount = shapeType is GlbShapeType.MultiConvex ? GetShapeCount(part) : -1;

            var meshTransform = part.GlobalTransform().TransformedBy(offset);
            var shapeTransform = meshTransform.Scaled(Vector3.One * shapeScaleMultiplier);
            var shapes = CreateShapes(part.Mesh, shapeTransform, shapeType, shapeCount);
            if (shapeType is GlbShapeType.MultiConvex) SetShapeCount?.Invoke(part, shapes.Length);

            if (shapes.Length is 0 or 1) AddShape(mode is MeshMode.Dynamic ? part.Copy() : part, shapes.SingleOrDefault());
            else shapes.ForEach(shape => AddShape(mode is MeshMode.Dynamic ? part.Clip((CollisionShape3D)shape) : part, shape, multi: true));

            void AddShape(MeshInstance3D mesh, Node3D shape, bool multi = false)
            {
                var name = partName ??= mesh.Name;
                if (multi) name += "1";

                if (shape is null)
                {
                    AddMesh(name, root);
                    SetMetaData();
                    if (mode is MeshMode.Dynamic)
                        OnPartAdded?.Invoke(mesh);
                }
                else
                {
                    AddShape(name, root);
                    AddMesh("Mesh", shape);
                    SetMetaData();
                    OnPartAdded?.Invoke(shape);
                }

                void AddMesh(string name, Node parent)
                {
                    if (mode is MeshMode.Dynamic)
                    {
                        mesh.Name = name;
                        parent.AddChild(mesh, owner);
                        mesh.GlobalTransform(meshTransform);
                    }
                }

                void AddShape(string name, Node parent)
                {
                    shape.Name = name;
                    parent.AddChild(shape, owner);
                    shape.GlobalTransform(shape.Transform);
                    if (disableShapes) (shape as CollisionShape3D)?.Disabled = true;
                }

                void SetMetaData()
                {
                    var aabb = mesh.GetAabb().TransformedBy(meshTransform);
                    var mass = aabb.Volume * massMultiplier;

                    if (shape is not null)
                    {
                        shape.SetMeta(Aabb, aabb);
                        shape.SetMeta(Mass, mass);
                        shape.SetMeta(Mesh, shape.GetPathTo(mesh));
                    }

                    if (mode is MeshMode.Dynamic)
                    {
                        mesh.SetMeta(Aabb, aabb);
                        mesh.SetMeta(Mass, mass);
                    }

                    rootAabb = rootAabb is null ? aabb : rootAabb.Value.Merge(aabb);
                    rootMass += mass;
                }
            }
        }

        void AddBoundingShape()
        {
            if (rootAabb is null) return;
            var shape = SimpleShape(rootAabb.Value, boundingShape);
            shape.Name = "Shape";
            root.AddChild(shape, owner);
            GLB.AddPart(shape);
        }
    }
}
