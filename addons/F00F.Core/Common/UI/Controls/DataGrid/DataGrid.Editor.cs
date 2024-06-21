#if TOOLS
namespace F00F;

partial class DataGrid
{
    public sealed override void _Notification(int what)
    {
        Editor.OnSave(what, OnSave);

        void OnSave()
        {
            //Editor.DoPreSaveReset(this, PropertyName);
        }
    }
}
#endif
