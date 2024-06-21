using System.Diagnostics;
using Godot;

namespace F00F;

public static partial class GLB
{
    public static bool Save(string path, Node scene, out Error err, out string msg)
    {
        Debug.Assert(path.GetExtension() is "glb" or "gltf");

        var doc = new GltfDocument();
        var state = new GltfState();

        err = doc.AppendFromScene(scene, state);
        if (err is Error.Ok)
        {
            err = doc.WriteToFilesystem(state, path);
            if (err is Error.Ok)
            {
                msg = null;
                return true;
            }
        }

        msg = $"Error ({err}) saving {path.GetExtension()} [{path}]";
        return false;
    }

    public static bool Load(string path, out Node scene, out Error err, out string msg)
    {
        Debug.Assert(path.GetExtension() is "glb" or "gltf");

        var doc = new GltfDocument();
        var state = new GltfState();

        err = doc.AppendFromFile(path, state);
        if (err is Error.Ok)
        {
            msg = null;
            scene = doc.GenerateScene(state);
            return true;
        }

        msg = $"Error ({err}) loading {path.GetExtension()} [{path}]";
        scene = null;
        return false;
    }
}
