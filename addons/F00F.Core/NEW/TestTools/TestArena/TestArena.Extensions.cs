using Godot;

namespace F00F;

public static class TestArena_Extensions
{
    public static void Clamp(this TestArena self, Node3D node, float radius)
    {
        var pos = node.GlobalPosition;
        var extent = self.Config.FloorSize * .5f;
        pos.X = Mathf.Clamp(pos.X, -extent, extent);
        pos.Y = Mathf.Max(pos.Y, radius);
        pos.Z = Mathf.Clamp(pos.Z, -extent, extent);
        node.Position = pos;
    }
}
