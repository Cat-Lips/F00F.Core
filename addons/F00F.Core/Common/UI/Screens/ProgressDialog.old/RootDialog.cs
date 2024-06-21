using Godot;

namespace F00F;

// Obsolete, use RootPopup instead (popup features with dialog border)

[Tool]
public partial class RootDialog : Window
{
    public RootDialog()
        => Visible = Editor.IsEditor;

    protected virtual void OnReady() { }
    public sealed override void _Ready()
    {
        CloseRequested += Hide;
        OnReady();
    }
}
