#if TOOLS
using Godot;

namespace F00F;

[Tool]
public partial class PhysicsImport : EditorScenePostImport
{
    public sealed override GodotObject _PostImport(Node scene)
    {
        var path = $"{GetSourceFile()}.physics";
        var config = GlbOptions.Load(path);

        scene = GLB.ApplyPhysics(scene, config);
        config.Save(path);
        return scene;
    }
}
#endif
