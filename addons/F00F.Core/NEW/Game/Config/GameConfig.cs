using Godot;

namespace F00F.NEW;

[Tool, GlobalClass]
public partial class GameConfig : CustomResource
{
    [Export] public bool QuickHelp { get; set; } = true;
    [Export] public bool QuickExit { get; set; } = true;
}
