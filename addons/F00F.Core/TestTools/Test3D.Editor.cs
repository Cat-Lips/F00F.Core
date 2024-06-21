#if TOOLS
using Godot.Collections;

namespace F00F;

public partial class Test3D
{
    protected virtual bool ValidateProperty(Dictionary property) => false;
    public sealed override void _ValidateProperty(Dictionary property)
    {
        if (ValidateProperty(property)) return;
    }

    protected virtual void _OnEditorSave() { }
    protected sealed override void OnEditorSave()
    {
        Camera.SkipSave();
        _OnEditorSave();
    }
}
#endif
