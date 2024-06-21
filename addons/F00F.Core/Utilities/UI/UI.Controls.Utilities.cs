using System.Collections.Generic;
using System.Linq;

namespace F00F;

public static partial class UI
{
    public static bool IsSceneFile(string path)
        => SceneFiles().Any(path.EndsWithN);

    public static bool IsModelFile(string path)
        => ModelFiles().Any(path.EndsWithN);

    private static IEnumerable<string> SceneFiles()
        => _SceneFiles().Concat(ModelFiles());

    private static IEnumerable<string> _SceneFiles()
    {
        yield return ".scn";
        yield return ".tscn";
    }

    private static IEnumerable<string> ModelFiles()
    {
        yield return ".glb";
        yield return ".gltf";

        yield return ".fbx";
        yield return ".dae";
        yield return ".obj";
        yield return ".blend";
    }
}
