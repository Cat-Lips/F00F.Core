#if TOOLS
namespace F00F;

public partial class Test3D
{
    protected virtual void _OnEditorSave() { }
    protected sealed override void OnEditorSave()
    {
        Editor.DoPreSaveResetField(Camera, Camera3D.PropertyName.Config);
        _OnEditorSave();
    }
}
#endif
