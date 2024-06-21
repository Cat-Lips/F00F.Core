using System;
using System.Linq;
using Godot;

namespace F00F;

public static partial class GLB
{
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
}
