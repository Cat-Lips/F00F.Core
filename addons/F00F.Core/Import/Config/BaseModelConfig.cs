using Godot;

namespace F00F;

[Tool, GlobalClass]
public partial class BaseModelConfig : CustomResource
{
    [ExportCategory("ModelConfig")]

    [Export] public PackedScene Scene { get; set => this.Set(ref field, value, notify: true); }

    [Export] public GlbRotate Rotation { get; set => this.Set(ref field, value); } = GlbRotate.Flip;
    [Export(PropertyHint.Range, "0,100,or_greater")] public float MassMultiplier { get; set => this.Set(ref field, value); } = 100;
    [Export(PropertyHint.Range, "0,10,or_greater")] public float MeshScaleMultiplier { get; set => this.Set(ref field, value); } = 1;
    [Export(PropertyHint.Range, "0,1")] public float ShapeScaleMultiplier { get; set => this.Set(ref field, value); } = 1;
}
