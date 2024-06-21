#if TOOLS
namespace F00F;

public partial class KeyEdit
{
    public sealed override void _Notification(int what)
    {
        Editor.OnSave(what, OnSave);

        void OnSave()
        {
            Clear();
            Editor.DoPreSaveReset(() => this.SetItems(Keys));
            return;
        }
    }
}
#endif
