using Godot;

namespace F00F;

[Tool, GlobalClass]
public partial class Camera2DConfig : CustomResource
{
    [ExportGroup("Movement")]
    [Export] public float Acceleration { get; set; } = 100;
    [Export] public float Deceleration { get; set; } = 35;
    [Export] public float MaxSpeed { get; set; } = 20;
    [Export] public float Lerp { get; set; } = 32;
}
