using System;
using System.Linq;
using Godot;
using static Godot.Vector3;

namespace F00F;

public static class MeshExtensions_Shapes
{
    #region Complex Shapes

    public static CollisionShape3D[] CreateShapes(this Mesh mesh, in Transform3D xform, int max = -1)
    {
        const int DefaultShapeCount = int.MaxValue;

        return mesh.CreateShapes(xform, x => x.CreateMultipleConvexCollisions(NewSettings()));

        MeshConvexDecompositionSettings NewSettings() => new()
        {
            MaxConcavity = Const.TinyFloat,
            MaxConvexHulls = (uint)(max < 0 ? DefaultShapeCount : max),
        };
    }

    public static CollisionShape3D CreateShape(this Mesh mesh, in Transform3D xform, bool simplify = false)
        => mesh.CreateShapes(xform, x => x.CreateConvexCollision(simplify: simplify)).Single();

    public static CollisionShape3D CreateShapeAsTrimesh(this Mesh mesh, in Transform3D xform)
        => mesh.CreateShapes(xform, x => x.CreateTrimeshCollision()).Single();

    #region Private

    private static CollisionShape3D[] CreateShapes(this Mesh mesh, in Transform3D xform, Action<MeshInstance3D> CreateChildCollider)
    {
        var tmp = new MeshInstance3D { Mesh = mesh };
        CreateChildCollider(tmp);
        var body = tmp.GetChildren<StaticBody3D>().Single();
        var shapes = body.GetChildren<CollisionShape3D>().ToArray();
        foreach (var x in shapes) x.Transform = xform;
        shapes.ForEach(body.RemoveChild);
        tmp.QueueFree();
        return shapes;
    }

    #endregion

    #endregion

    #region Simple Shapes

    public static CollisionShape3D CreateBoxShape(this Mesh mesh, in Transform3D xform) => mesh.GetAabb().TransformedBy(xform).CreateBoxShape();
    public static CollisionShape3D CreateSphereShape(this Mesh mesh, in Transform3D xform) => mesh.GetAabb().TransformedBy(xform).CreateSphereShape();
    public static CollisionShape3D CreateCapsuleShape(this Mesh mesh, in Transform3D xform, Axis? axis = null) => mesh.GetAabb().TransformedBy(xform).CreateCapsuleShape(axis);
    public static CollisionShape3D CreateCylinderShape(this Mesh mesh, in Transform3D xform, Axis axis = Axis.Y) => mesh.GetAabb().TransformedBy(xform).CreateCylinderShape(axis);
    public static CollisionShape3D CreateRayShape(this Mesh mesh, in Transform3D xform, Axis axis = Axis.Y, bool slide = false) => mesh.GetAabb().TransformedBy(xform).CreateRayShape(axis, slide);
    public static VehicleWheel3D CreateWheelShape(this Mesh mesh, in Transform3D xform, Axis axis = Axis.X) => mesh.GetAabb().TransformedBy(xform).CreateWheelShape(axis);
    public static RayCast3D CreateRayCast(this Mesh mesh, in Transform3D xform, Axis axis = Axis.Y) => mesh.GetAabb().TransformedBy(xform).CreateRayCast(axis);

    public static CollisionShape3D CreateCylinderShapeLong(this Mesh mesh, in Transform3D xform) => mesh.GetAabb().TransformedBy(xform).CreateCylinderShapeLong();
    public static CollisionShape3D CreateCylinderShapeShort(this Mesh mesh, in Transform3D xform) => mesh.GetAabb().TransformedBy(xform).CreateCylinderShapeShort();

    #endregion
}
