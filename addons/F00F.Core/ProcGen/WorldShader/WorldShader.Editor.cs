#if TOOLS
namespace F00F;

public partial class WorldShader
{
    public sealed override void _Notification(int what)
    {
        if (Editor.OnPreSave(what))
        {
            if (this.IsEditedSceneRoot())
            {
                Editor.DoPreSaveResetField(this, PropertyName.WorldConfig);
                Editor.DoPreSaveResetField(this, PropertyName.ChunkConfig);
            }

            return;
        }

        if (Editor.OnPostSave(what))
            Editor.DoPostSaveRestore();
    }
}
#endif
