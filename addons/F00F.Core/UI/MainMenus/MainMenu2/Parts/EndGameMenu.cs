using Godot;

namespace F00F;

public partial class EndGameMenu : Container
{
    public Button Continue => field ??= (Button)GetNode("%Continue");
    public RichTextLabel Message => field ??= (RichTextLabel)GetNode("%Message");
    public Button Quit => field ??= (Button)GetNode("%Quit");
}
