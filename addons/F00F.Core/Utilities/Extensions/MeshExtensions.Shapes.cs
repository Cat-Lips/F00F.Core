using System;
using System.Linq;
using Godot;
using static Godot.Vector3;

namespace F00F
{
    public static class MeshExtensions_Shapes
    {
        #region Complex Shapes

        public static CollisionShape3D[] CreateShapes(this Mesh mesh, in Transform3D xform, int max = 0)
        {
            const int DefaultShapeCount = int.MaxValue;

            return mesh.CreateShapes(xform, x => x.CreateMultipleConvexCollisions(NewSettings()));

            MeshConvexDecompositionSettings NewSettings() => new()
            {
                MaxConcavity = Const.TinyFloat,
                MaxConvexHulls = (uint)(max <= 0 ? DefaultShapeCount : max),
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

        public static CollisionShape3D CreateCylinderShapeLong(this Mesh mesh, in Transform3D xform) { var bb = mesh.GetAabb().TransformedBy(xform); return bb.CreateCylinderShape(bb.GetLongestAxisIndex()); }
        public static CollisionShape3D CreateCylinderShapeShort(this Mesh mesh, in Transform3D xform) { var bb = mesh.GetAabb().TransformedBy(xform); return bb.CreateCylinderShape(bb.GetShortestAxisIndex()); }

        #region Private

        private static CollisionShape3D CreateBoxShape(this in Aabb bb)
            => new() { Position = bb.GetCenter(), Shape = BoxShape(bb.Size) };

        private static CollisionShape3D CreateSphereShape(this in Aabb bb)
            => new() { Position = bb.GetCenter(), Shape = SphereShape(bb.GetLongestAxisSize() * .5f) };

        private static CollisionShape3D CreateCapsuleShape(this in Aabb bb, Axis? axis)
        {
            var (height, radius, rotation) = GetRadialDimensions(bb.Size, axis ?? bb.GetLongestAxisIndex(), from: Axis.Y);
            return new() { Position = bb.GetCenter(), Rotation = rotation, Shape = CapsuleShape(height, radius) };
        }

        private static CollisionShape3D CreateCylinderShape(this in Aabb bb, Axis axis)
        {
            var (height, radius, rotation) = GetRadialDimensions(bb.Size, axis, from: Axis.Y);
            return new() { Position = bb.GetCenter(), Rotation = rotation, Shape = CylinderShape(height, radius) };
        }

        private static CollisionShape3D CreateRayShape(this in Aabb bb, Axis axis, bool slide = false)
        {
            var (height, _, rotation) = GetRadialDimensions(bb.Size, axis, from: Axis.Z);
            return new() { Position = bb.GetCenter(), Rotation = rotation, Shape = RayShape(height * .5f, slide) };
        }

        private static VehicleWheel3D CreateWheelShape(this in Aabb bb, Axis axis)
        {
            var (_, radius, rotation) = GetRadialDimensions(bb.Size, axis, from: Axis.X);
            return new VehicleWheel3D { Position = bb.GetCenter(), Rotation = rotation, WheelRadius = radius };
        }

        private static RayCast3D CreateRayCast(this in Aabb bb, Axis axis)
        {
            var (_, radius, rotation) = GetRadialDimensions(bb.Size, axis, from: Axis.Y);
            return new() { Position = bb.GetCenter(), Rotation = rotation, TargetPosition = Down * radius };
        }

        private static BoxShape3D BoxShape(in Vector3 size) => new() { Size = size };
        private static SphereShape3D SphereShape(float radius) => new() { Radius = radius };
        private static CapsuleShape3D CapsuleShape(float height, float radius) => new() { Height = height, Radius = radius };
        private static CylinderShape3D CylinderShape(float height, float radius) => new() { Height = height, Radius = radius };
        private static SeparationRayShape3D RayShape(float length, bool slide = false) => new() { Length = length, SlideOnSlope = slide };

        private static (float Height, float Radius, Vector3 Rotation) GetRadialDimensions(in Vector3 size, Axis axis, Axis from)
        {
            return axis switch
            {
                Axis.X => (size.X, Mathf.Max(size.Y, size.Z) * .5f, RotX(from)),
                Axis.Y => (size.Y, Mathf.Max(size.Z, size.X) * .5f, RotY(from)),
                Axis.Z => (size.Z, Mathf.Max(size.X, size.Y) * .5f, RotZ(from)),
                _ => throw new NotImplementedException(),
            };

            static Vector3 RotX(Axis from) => from switch
            {
                Axis.X => Zero,
                Axis.Y => new(0, 0, Const.Deg90),
                Axis.Z => new(0, Const.Deg90, 0),
                _ => throw new NotImplementedException(),
            };

            static Vector3 RotY(Axis from) => from switch
            {
                Axis.X => new(0, 0, Const.Deg90),
                Axis.Y => Zero,
                Axis.Z => new(Const.Deg90, 0, 0),
                _ => throw new NotImplementedException(),
            };

            static Vector3 RotZ(Axis from) => from switch
            {
                Axis.X => new(0, Const.Deg90, 0),
                Axis.Y => new(Const.Deg90, 0, 0),
                Axis.Z => Zero,
                _ => throw new NotImplementedException(),
            };
        }

        #endregion

        #endregion
    }
}
