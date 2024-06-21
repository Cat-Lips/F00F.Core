#if TOOLS
namespace F00F;

public partial class Root3D
{
    public override void _Notification(int what)
    {
        if (Editor.OnPreSave(what))
            Source.OnPreSave();
        else if (Editor.OnPostSave(what))
            Source.OnPostSave();
    }
}
#endif
