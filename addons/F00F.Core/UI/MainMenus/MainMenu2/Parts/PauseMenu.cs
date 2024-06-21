using Godot;

namespace F00F;

[Tool]
public partial class PauseMenu : Container
{
    public Button Resume => field ??= GetNode<Button>("%Resume");
    public RichTextLabel Message => field ??= GetNode<RichTextLabel>("%Message");
    public Button Quit => field ??= GetNode<Button>("%Quit");
}
