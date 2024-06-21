using Godot;

namespace F00F;

[Tool, GlobalClass]
public partial class ModelConfig : CustomResource
{
    [Export] public PackedScene Scene { get; set => this.Set(ref field, value); }
    [Export] public GlbRotate Rotation { get; set => this.Set(ref field, value); } = GlbRotate.Flip;
    [Export(PropertyHint.Range, "0,1,or_greater")] public float MassMultiplier { get; set => this.Set(ref field, value); } = 1;
    [Export(PropertyHint.Range, "0,1,or_greater")] public float ScaleMultiplier { get; set => this.Set(ref field, value); } = 1;
}
