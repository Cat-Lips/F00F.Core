using Godot;

namespace F00F;

[Tool]
public partial class PauseMenu : Container
{
    public Button Resume => field ??= (Button)GetNode("%Resume");
    public RichTextLabel Message => field ??= (RichTextLabel)GetNode("%Message");
    public Button Quit => field ??= (Button)GetNode("%Quit");
}
