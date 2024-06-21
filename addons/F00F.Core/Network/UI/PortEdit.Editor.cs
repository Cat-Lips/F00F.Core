#if TOOLS
using Godot;

namespace F00F;

public partial class PortEdit
{
    public sealed override void _Notification(int what)
    {
        if (Editor.OnPreSave(what))
        {
            Editor.DoPreSaveReset(this, Range.PropertyName.MinValue);
            Editor.DoPreSaveReset(this, Range.PropertyName.MaxValue, 100.0);
            Editor.DoPreSaveReset(this, Range.PropertyName.Value);
            return;
        }

        if (Editor.OnPostSave(what))
            Editor.DoPostSaveRestore();
    }
}
#endif
