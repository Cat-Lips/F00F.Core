using Godot;

namespace F00F.Core;

[Tool, GlobalClass]
public partial class PartConfig : CustomResource
{
    [Export] public NodePath Path { get; set => this.Set(ref field, value); }
    [Export] public string Name { get; set => this.Set(ref field, value); }
    [Export] public GlbShapeType Shape { get; set => this.Set(ref field, value, notify: true); } = GlbShapeType.SimpleConvex;
    [Export] public int MaxShapes { get; set => this.Set(ref field, value); }

    [Export] public bool Visible { get; set => this.Set(ref field, value); } = true;
    [Export] public bool Disabled { get; set => this.Set(ref field, value); }
}
