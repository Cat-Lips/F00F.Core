using Godot;

namespace F00F;

public static class AabbExtensions_Transforms
{
    public static Aabb Scaled(this Aabb source, float scale)
        => source.TransformedBy(Transform3D.Identity.Scaled(Vector3.One * scale));
}
