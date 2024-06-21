using Godot;

namespace F00F;

[Tool]
public partial class GUI : CanvasLayer
{
    public QuickHelp QuickHelp => field ??= (QuickHelp)GetNode("QuickHelp");
    public PlayerList PlayerList => field ??= (PlayerList)GetNode("PlayerList");

    public IPlayerData PlayerData => field ??= new PlayerData();

    public GUI()
        => Layer = Const.CanvasLayer.GUI;
}
