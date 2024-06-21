#if TOOLS
namespace F00F
{
    public partial class ModelView
    {
        public override void _Notification(int what)
        {
            if (Editor.OnPreSave(what))
            {
                Editor.DoPreSaveReset(View.Size, x => View.Size = x);
                Editor.DoPreSaveReset(Camera.Position, x => Camera.Position = x);
                return;
            }

            if (Editor.OnPostSave(what))
                Editor.DoPostSaveRestore();
        }
    }
}
#endif
