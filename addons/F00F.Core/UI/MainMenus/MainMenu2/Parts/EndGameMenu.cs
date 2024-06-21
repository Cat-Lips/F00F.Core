using Godot;

namespace F00F;

public partial class EndGameMenu : Container
{
    public Button Continue => field ??= GetNode<Button>("%Continue");
    public RichTextLabel Message => field ??= GetNode<RichTextLabel>("%Message");
    public Button Quit => field ??= GetNode<Button>("%Quit");
}
