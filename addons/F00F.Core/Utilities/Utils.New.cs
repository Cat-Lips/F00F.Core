using System.Diagnostics;
using Godot;

namespace F00F;

public static class New
{
    public static ShaderMaterial ShaderMaterial() => new() { Shader = new() };

    public static PlaneMesh PlaneMesh(int size, int lod, Material material)
    {
        Debug.Assert(size.IsPo2());

        lod = lod <= 0 ? size - 1
            : size / (int)Mathf.Pow(2, lod) - 1;

        return new()
        {
            Size = Vector2.One * size,
            SubdivideDepth = lod,
            SubdivideWidth = lod,
            Material = material,
        };

    }
}
