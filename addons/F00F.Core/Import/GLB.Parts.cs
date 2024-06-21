using System;
using System.Linq;
using Godot;

namespace F00F;

public static partial class GLB
{
    public static void AddParts(
        Node root,
        Node source,
        GlbRotate rotation,
        float massMultiplier,
        float scaleMultiplier,
        Func<MeshInstance3D, string> GetPartName = null,
        Func<MeshInstance3D, GlbShapeType> GetShapeType = null,
        Func<MeshInstance3D, int> GetShapeCount = null,
        Action<MeshInstance3D, int> SetShapeCount = null,
        Action<Node3D> OnPartAdded = null)
    {
        GetPartName ??= Default.GetPartName;
        GetShapeType ??= Default.GetShapeType;
        GetShapeCount ??= Default.GetShapeCount;

        float rootMass = default;
        Aabb? rootAabb = default;
        var offset = GetOffset(rotation, scaleMultiplier);

        source?.RecurseChildren<MeshInstance3D>(self: true).ForEach(AddPart);

        SetRootMass(root, rootMass);
        SetRootAabb(root, rootAabb ?? default);

        void AddPart(MeshInstance3D part)
        {
            var partName = GetPartName(part);
            var shapeType = GetShapeType(part);
            var shapeCount = shapeType is GlbShapeType.MultiConvex ? GetShapeCount(part) : -1;

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
                    root.AddChild(mesh, own: true);
                    mesh.GlobalTransform(xform);

                    SetMeshMass();

                    OnPartAdded?.Invoke(mesh);
                }
                else
                {
                    shape.Name = name;
                    root.AddChild(shape, own: true);
                    shape.GlobalTransform(shape.Transform);

                    mesh.Name = "Mesh";
                    shape.AddChild(mesh, own: true);
                    mesh.GlobalTransform(xform);

                    SetMeshMass();

                    OnPartAdded?.Invoke(shape);
                }

                void SetMeshMass()
                {
                    var aabb = mesh.GetAabb().TransformedBy(xform);
                    var mass = aabb.Volume * massMultiplier;
                    shape?.SetMeta(Aabb, aabb);
                    shape?.SetMeta(Mass, mass);
                    mesh.SetMeta(Aabb, aabb);
                    mesh.SetMeta(Mass, mass);
                    rootAabb = rootAabb is null ? aabb : rootAabb.Value.Merge(aabb);
                    rootMass += mass;
                }
            }
        }
    }
}
