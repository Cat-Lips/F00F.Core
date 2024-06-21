#if TOOLS
using Godot;

namespace F00F.NEW;

public partial class SessionFilter
{
    public sealed override void _Notification(int what)
    {
        Editor.OnSave(what, OnSave);

        void OnSave()
        {
            Editor.DoPreSaveReset(Title, Button.PropertyName.Text);
            Editor.DoPreSaveReset(Refresh, Button.PropertyName.Text);
        }
    }
}
#endif
