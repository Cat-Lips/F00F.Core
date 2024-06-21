using System;
using System.Linq;
using Godot;
using static Godot.Vector3;

namespace F00F
{
    public static class MeshExtensions_Shapes
    {
        public static CollisionShape3D[] CreateShapes(this Mesh mesh, int max = 0)
        {
            const int HardLimit = int.MaxValue;

            // Bit of a hack until method moved to Mesh
            var tmp = new MeshInstance3D { Mesh = mesh };
            tmp.CreateMultipleConvexCollisions(NewSettings());
            var body = tmp.GetChildren<StaticBody3D>().Single();
            var shapes = body.GetChildren<CollisionShape3D>().ToArray();
            shapes.ForEach(body.RemoveChild);
            tmp.QueueFree();
            return shapes;

            MeshConvexDecompositionSettings NewSettings() => new()
            {
                MaxConcavity = 0.001f,
                MaxConvexHulls = (uint)(max < 1 ? HardLimit : max),
            };
        }

        public static CollisionShape3D CreateBoxShape(this Mesh mesh) => mesh.GetAabb().CreateBoxShape();
        public static CollisionShape3D CreateSphereShape(this Mesh mesh) => mesh.GetAabb().CreateSphereShape();
        public static CollisionShape3D CreateCapsuleShape(this Mesh mesh, Axis axis) => mesh.GetAabb().CreateCapsuleShape(axis);
        public static CollisionShape3D CreateCylinderShape(this Mesh mesh, Axis axis) => mesh.GetAabb().CreateCylinderShape(axis);
        public static CollisionShape3D CreateRayShape(this Mesh mesh, Axis axis, bool slide = false) => mesh.GetAabb().CreateRayShape(axis, slide);
        public static VehicleWheel3D CreateWheelShape(this Mesh mesh) => mesh.GetAabb().CreateWheelShape();
        public static RayCast3D CreateRayCast(this Mesh mesh, Axis axis) => mesh.GetAabb().CreateRayCast(axis);

        public static CollisionShape3D CreateBoxShape(this in Aabb bb)
            => new() { Position = bb.GetCenter(), Shape = BoxShape(bb.Size) };

        public static CollisionShape3D CreateSphereShape(this in Aabb bb)
            => new() { Position = bb.GetCenter(), Shape = SphereShape(bb.GetLongestAxisSize() * .5f) };

        public static CollisionShape3D CreateCapsuleShape(this in Aabb bb, Axis axis)
        {
            var (height, radius, rotation) = GetRadialDimensions(bb.Size, axis);
            return new() { Position = bb.GetCenter(), Rotation = rotation, Shape = CapsuleShape(height, radius) };
        }

        public static CollisionShape3D CreateCylinderShape(this in Aabb bb, Axis axis)
        {
            var (height, radius, rotation) = GetRadialDimensions(bb.Size, axis);
            return new() { Position = bb.GetCenter(), Rotation = rotation, Shape = CylinderShape(height, radius) };
        }

        public static CollisionShape3D CreateRayShape(this in Aabb bb, Axis axis, bool slide = false)
        {
            var (height, _, rotation) = GetRadialDimensions(bb.Size, axis);
            return new() { Position = bb.GetCenter(), Rotation = rotation, Shape = RayShape(height * .5f, slide) };
        }

        public static VehicleWheel3D CreateWheelShape(this in Aabb bb)
        {
            var (_, radius, rotation) = GetWheelDimensions(bb.Size, bb.GetShortestAxisIndex());
            return new VehicleWheel3D { Position = bb.GetCenter(), Rotation = rotation, WheelRadius = radius };
        }

        public static RayCast3D CreateRayCast(this in Aabb bb, Axis axis)
        {
            var (height, _, rotation) = GetRadialDimensions(bb.Size, axis);
            return new() { Position = bb.GetCenter(), Rotation = rotation, TargetPosition = new(0, -height * .5f, 0) };
        }

        private static BoxShape3D BoxShape(in Vector3 size) => new() { Size = size };
        private static SphereShape3D SphereShape(float radius) => new() { Radius = radius };
        private static CapsuleShape3D CapsuleShape(float height, float radius) => new() { Height = height, Radius = radius };
        private static CylinderShape3D CylinderShape(float height, float radius) => new() { Height = height, Radius = radius };
        private static SeparationRayShape3D RayShape(float length, bool slide = false) => new() { Length = length, SlideOnSlope = slide };

        private static (float Height, float Radius, Vector3 Rotation) GetRadialDimensions(in Vector3 size, Axis axis)
        {
            return axis switch
            {
                Axis.X => (size.X, Mathf.Max(size.Y, size.Z) * .5f, new(0, 0, Const.Deg90)),
                Axis.Y => (size.Y, Mathf.Max(size.Z, size.X) * .5f, new(Const.Deg90, 0, 0)),
                Axis.Z => (size.Z, Mathf.Max(size.X, size.Y) * .5f, Zero),
                _ => throw new NotImplementedException(),
            };
        }

        private static (float Width, float Radius, Vector3 Rotation) GetWheelDimensions(in Vector3 size, Axis axis)
        {
            return axis switch
            {
                Axis.X => (size.X, Mathf.Max(size.Y, size.Z) * .5f, Zero),
                Axis.Y => (size.Y, Mathf.Max(size.Z, size.X) * .5f, new(0, 0, Const.Deg90)),
                Axis.Z => (size.Z, Mathf.Max(size.X, size.Y) * .5f, new(0, Const.Deg90, 0)),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
