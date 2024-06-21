namespace F00F;

public partial class GUI
{
    public sealed override void _Ready()
        => App.InitQuit(this);

#if !TOOLS
    public sealed override void _Notification(int what)
        => App.NotifyQuit(this, what);
#endif
}
