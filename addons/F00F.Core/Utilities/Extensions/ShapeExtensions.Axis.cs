using Godot;

namespace F00F;

public static class ShapeExtensions_Axis
{
    public static float GetHorizontalRadius(this Shape3D shape)
    {
        return shape switch
        {
            SphereShape3D s => s.Radius,
            CapsuleShape3D c => c.Radius,
            CylinderShape3D c => c.Radius,
            BoxShape3D b => ComputeBoxDiagonal(b),
            ConvexPolygonShape3D c => ComputeConvexRadius(c),
            ConcavePolygonShape3D c => ComputeConcaveRadius(c),
            _ => throw new System.NotImplementedException(),
        };
    }

    private static float ComputeBoxDiagonal(BoxShape3D b)
        => Mathf.Sqrt(
            b.Size.X * 0.5f * b.Size.X * 0.5f +
            b.Size.Z * 0.5f * b.Size.Z * 0.5f);

    private static float ComputeConvexRadius(ConvexPolygonShape3D c)
    {
        var max = 0f;
        foreach (var v in c.Points)
        {
            var h = v.X * v.X + v.Z * v.Z;
            if (h > max) max = h;
        }
        return Mathf.Sqrt(max);
    }

    private static float ComputeConcaveRadius(ConcavePolygonShape3D c)
    {
        var max = 0f;
        foreach (var v in c.Data)
        {
            var h = v.X * v.X + v.Z * v.Z;
            if (h > max) max = h;
        }
        return Mathf.Sqrt(max);
    }
}
