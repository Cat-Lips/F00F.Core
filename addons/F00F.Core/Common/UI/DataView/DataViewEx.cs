using Godot;

namespace F00F;

[Tool]
public partial class DataViewEx : DataView
{
    private Label _Title => field ??= GetNode<Label>("%Title");
    protected string Title { get => _Title.Text; set => _Title.Text = value; }
}
