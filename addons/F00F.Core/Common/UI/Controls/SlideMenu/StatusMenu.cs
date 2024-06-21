using Godot;

namespace F00F;

[Tool]
public partial class StatusMenu : SlideMenu
{
    private Separator StatusSep => field ??= (Separator)GetNode("%StatusSep");
    private StatusBar StatusBar => field ??= (StatusBar)GetNode("%StatusBar");

    private Status StatusType { get; set; }
    private string StatusText { get; set; }

    protected void SetStatus(Status type, string text)
    {
        StatusType = type;
        StatusText = text;
        SetStatusVisibility();
        StatusBar.SetStatus(type, text);
    }

    protected void ClearStatus()
    {
        StatusType = default;
        StatusText = default;
        StatusBar.ClearStatus();
    }

    protected sealed override void OnReady1()
    {
        SetStatusVisibility();
        Items.VisibilityChanged += SetStatusVisibility;
    }

    private void SetStatusVisibility()
    {
        var hasStatus = StatusText is not null;
        var isImportant = StatusType is Status.Error or Status.Warn;
        var showStatus = hasStatus && (isImportant || Items.Visible);

        StatusSep.Visible = showStatus;
        StatusBar.Visible = showStatus;
    }
}
