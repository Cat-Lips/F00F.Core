#if TOOLS
namespace F00F
{
    public partial class Test3D
    {
        public override void _Notification(int what)
        {
            if (Editor.OnPreSave(what))
            {
                if (this.IsEditedSceneRoot())
                    Editor.DoPreSaveReset(Camera, Camera.PropertyName._config);
                return;
            }

            if (Editor.OnPostSave(what))
                Editor.DoPostSaveRestore();
        }
    }
}
#endif
