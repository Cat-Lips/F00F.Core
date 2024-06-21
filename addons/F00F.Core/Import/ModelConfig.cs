using Godot;

namespace F00F;

[Tool, GlobalClass]
public partial class ModelConfig : CustomResource
{
    [Export] public NodePath Source { get; set => this.Set(ref field, value, notify: true); }
    [Export] public PackedScene Scene { get; set => this.Set(ref field, value, notify: true); }

    [Export] public GlbRotate Rotation { get; set => this.Set(ref field, value); } = GlbRotate.Flip;
    [ExportToolButton(nameof(OnSourceUpdated))] private Callable OnSourceUpdated => Callable.From(EmitChanged);
    [Export(PropertyHint.Range, "0,100,or_greater")] public float MassMultiplier { get; set => this.Set(ref field, value); } = 100;
    [Export(PropertyHint.Range, "0,10,or_greater")] public float ScaleMultiplier { get; set => this.Set(ref field, value); } = 1;
    [Export(PropertyHint.Range, "0,.5,.01")] public float ShapeReductionRatio { get; set => this.Set(ref field, value); }
}
