using Godot;

namespace F00F;

[Tool]
public partial class DataViewEx : DataView
{
    protected Label Title => field ??= (Label)GetNode("%Title");
    protected Control Buttons => field ??= (Control)GetNode("%Buttons");
    public string TitleText { get => Title.Text; set => Title.Text = value; }
}
