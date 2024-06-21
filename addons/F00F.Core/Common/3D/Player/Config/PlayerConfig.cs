using Godot;

namespace F00F;

[Tool, GlobalClass]
public partial class PlayerConfig : CustomResource
{
    [Export] public float MovementSpeed { get; set => this.Set(ref field, value); } = 30;
    [Export] public float JumpStrength { get; set => this.Set(ref field, value); } = 10;
}
