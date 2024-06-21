using Godot;

namespace F00F;

public static class TextureLibraryExtensions
{
    public static void Set(this TextureLibrary textures, ShaderMaterial shader, params string[] @params)
    {
        if (@params.IsNullOrEmpty()) @params = DefaultParams();

        foreach (var param in @params)
        {

        }

        static string[] DefaultParams()
            => ["albedo_array", "normal_array", "height_array", "metallic_array", "roughness_array", "occlusion_array"];
    }
}
