using System;
using Godot;

namespace F00F;

[Tool]
public partial class StatusBar : Container
{
    #region Private

    private TextureRect Icon => field ??= (TextureRect)GetNode("Icon");
    private Label Text => field ??= (Label)GetNode("Text");
    private Button Action => field ??= (Button)GetNode("Action");

    #endregion

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

    public void SetAction(string text, Action action)
    {
        Action.Text = text;
        Action.Visible = true;

        this.action = action;
    }

    public void ClearAction()
    {
        Action.Text = default;
        Action.Visible = false;

        action = null;
    }

    private Action action;
    public sealed override void _Ready()
        => Action.Pressed += () => action?.Invoke();

    private readonly Control MyHistory = new VBoxContainer();
    public sealed override GodotObject _MakeCustomTooltip(string tt)
        => MyHistory;
}
