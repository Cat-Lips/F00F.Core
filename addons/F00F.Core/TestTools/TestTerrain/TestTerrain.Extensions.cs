using System.Collections.Generic;
using System.Linq;
using Godot;

namespace F00F;

public static class TestTerrain_Extensions
{
    public static void Clamp(this TestTerrain self, Node3D node, float radius)
    {
        var pos = node.GlobalPosition;

        var extents = self.Size / 2;
        pos.X = Mathf.Clamp(pos.X, -extents.X, extents.X);
        pos.Z = Mathf.Clamp(pos.Z, -extents.Y, extents.Y);

        var height = self.GetHeights(pos.X, pos.Z, radius).Max();
        pos.Y = Mathf.Max(pos.Y, height + radius);

        node.Position = pos;
    }

    public static IEnumerable<float> GetHeights(this TestTerrain self, float x, float z, float radius)
    {
        var rsqr = radius * radius;
        var r = Mathf.CeilToInt(radius);
        for (var dz = -r; dz <= r; ++dz)
            for (var dx = -r; dx <= r; ++dx)
                if (dx * dx + dz * dz <= rsqr)
                    yield return self.GetHeight(x + dx, z + dz);
    }
}
