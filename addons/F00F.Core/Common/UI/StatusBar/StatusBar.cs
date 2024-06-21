using Godot;

namespace F00F;

[Tool]
public partial class StatusBar : Container
{
    private TextureRect Icon => field ??= GetNode<TextureRect>("Icon");
    private Label Text => field ??= GetNode<Label>("Text");

    public void SetStatus(Status type, string msg)
    {
        Text.Text = msg;
        Icon.Texture = type.Icon();
        Text.SetFontColor(type.Color());

        MyHistory.AddChild(Duplicate());
    }

    public void ClearStatus()
    {
        Text.Text = null;
        Icon.Texture = null;
        Text.SetFontColor(null);

        MyHistory.RemoveChildren();
    }

    private readonly Control MyHistory = new VBoxContainer();
    public sealed override GodotObject _MakeCustomTooltip(string tt)
        => MyHistory;
}
