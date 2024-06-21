using Godot;

namespace F00F;

[Tool]
public partial class GUI : CanvasLayer
{
    public QuickHelp QuickHelp => field ??= GetNode<QuickHelp>("QuickHelp");
    public PlayerList PlayerList => field ??= GetNode<PlayerList>("PlayerList");

    public IPlayerData PlayerData => field ??= new PlayerData();

    public GUI()
        => Layer = Const.CanvasLayer.GUI;
}
