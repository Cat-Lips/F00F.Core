using Godot;

namespace F00F;

[Tool]
public partial class GUI : CanvasLayer
{
    public MainMenu MainMenu => field ??= GetNode<MainMenu>("MainMenu");
    public QuickHelp QuickHelp => field ??= GetNode<QuickHelp>("QuickHelp");
    public PlayerList PlayerList => field ??= GetNode<PlayerList>("PlayerList");

    public GUI()
        => Layer = Const.CanvasLayer.GUI;
}
