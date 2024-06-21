#if TOOLS
namespace F00F;

public partial class GUI
{
    public sealed override void _Notification(int what)
    {
        if (App.OnQuitNotify(this, what)) return;
        if (Editor.OnSave(what, OnPreSave)) return;

        void OnPreSave()
            => Editor.DoPreSaveReset(this, PropertyName.Layer, 1);
    }
}
#endif
