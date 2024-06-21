#if TOOLS
namespace F00F;

public partial class Camera
{
    public sealed override void _Notification(int what)
    {
        if (Editor.OnPreSave(what))
        {
            if (this.IsEditedSceneRoot())
                Editor.DoPreSaveResetField(this, PropertyName.Config);
            Editor.DoPreSaveResetOwner(this, where: GLB.IsPart);
            return;
        }

        if (Editor.OnPostSave(what))
            Editor.DoPostSaveRestore();
    }
}
#endif
