using Godot;

namespace F00F;

[Tool, GlobalClass]
public partial class Model : ModelConfig
{
    [Export] public GlbShapeType PartsShape { get; set => this.Set(ref field, value); }
    [Export] public GlbSimpleShapeType BoundingShape { get; set => this.Set(ref field, value); } = GlbSimpleShapeType.Sphere;

    public Model()
        => Rotation = GlbRotate.None;
}
