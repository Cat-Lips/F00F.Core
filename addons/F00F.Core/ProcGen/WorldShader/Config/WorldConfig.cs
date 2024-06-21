using Godot;

namespace F00F;

[Tool, GlobalClass]
public partial class WorldConfig : CustomResource
{
    public ShaderMaterial ShaderMaterial { get; }

    public WorldConfig()
        => ShaderMaterial = New.ShaderMaterial(this.TryLoadShader());
}
