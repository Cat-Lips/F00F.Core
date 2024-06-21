using System;
using Godot;
using static Godot.Vector3;

namespace F00F;

public static class AabbExtensions_Shapes
{
    public static CollisionShape3D CreateBoxShape(this in Aabb bb)
        => new() { Position = bb.GetCenter(), Shape = BoxShape(bb.Size) };

    public static CollisionShape3D CreateSphereShape(this in Aabb bb)
        => new() { Position = bb.GetCenter(), Shape = SphereShape(bb.GetLongestAxisSize() * .5f) };

    public static CollisionShape3D CreateCapsuleShape(this in Aabb bb, Axis? axis = null)
    {
        var (height, radius, rotation) = GetRadialDimensions(bb.Size, axis ?? bb.GetLongestAxisIndex(), from: Axis.Y);
        return new() { Position = bb.GetCenter(), Rotation = rotation, Shape = CapsuleShape(height, radius) };
    }

    public static CollisionShape3D CreateCylinderShape(this in Aabb bb, Axis axis)
    {
        var (height, radius, rotation) = GetRadialDimensions(bb.Size, axis, from: Axis.Y);
        return new() { Position = bb.GetCenter(), Rotation = rotation, Shape = CylinderShape(height, radius) };
    }

    public static CollisionShape3D CreateRayShape(this in Aabb bb, Axis axis, bool slide = false)
    {
        var (height, _, rotation) = GetRadialDimensions(bb.Size, axis, from: Axis.Z);
        return new() { Position = bb.GetCenter(), Rotation = rotation, Shape = RayShape(height * .5f, slide) };
    }

    public static VehicleWheel3D CreateWheelShape(this in Aabb bb, Axis axis = Axis.X)
    {
        var (_, radius, rotation) = GetRadialDimensions(bb.Size, axis, from: Axis.X);
        return new VehicleWheel3D { Position = bb.GetCenter(), Rotation = rotation, WheelRadius = radius };
    }

    public static RayCast3D CreateRayCast(this in Aabb bb, Axis axis)
    {
        var (_, radius, rotation) = GetRadialDimensions(bb.Size, axis, from: Axis.Y);
        return new() { Position = bb.GetCenter(), Rotation = rotation, TargetPosition = Down * radius };
    }

    public static CollisionShape3D CreateCylinderShapeLong(this in Aabb bb) => bb.CreateCylinderShape(bb.GetLongestAxisIndex());
    public static CollisionShape3D CreateCylinderShapeShort(this in Aabb bb) => bb.CreateCylinderShape(bb.GetShortestAxisIndex());

    #region Private

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
}
