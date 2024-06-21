using Godot;

namespace F00F;

[Tool]
public partial class DataViewEx : DataView
{
    protected Label Title => field ??= GetNode<Label>("%Title");
    protected Control Buttons => field ??= GetNode<Control>("%Buttons");
    public string TitleText { get => Title.Text; set => Title.Text = value; }
}
