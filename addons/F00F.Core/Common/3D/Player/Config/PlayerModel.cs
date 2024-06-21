using Godot;

namespace F00F;

[Tool, GlobalClass]
public partial class PlayerModel : ModelConfig
{
    [Export] public GlbShapeType PartsShape { get; set => this.Set(ref field, value); }
    [Export] public GlbSimpleShapeType BoundingShape { get; set => this.Set(ref field, value); } = GlbSimpleShapeType.Capsule;

    public PlayerModel()
        => Rotation = GlbRotate.None;
}
