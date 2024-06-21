#if TOOLS
using Godot;
using Godot.Collections;

namespace F00F;

public partial class IntEdit
{
    public sealed override void _ValidateProperty(Dictionary property)
    {
        Rounded = true;
        CustomArrowRound = true;
        if (Editor.SetDisplayOnly(property, SpinBox.PropertyName.Rounded)) return;
        if (Editor.SetDisplayOnly(property, SpinBox.PropertyName.CustomArrowRound)) return;
    }

    public sealed override void _Notification(int what)
    {
        Editor.OnSave(what, OnSave);

        void OnSave()
        {
            Rounded = true;
            CustomArrowRound = true;
            Editor.DoPreSaveReset(this, SpinBox.PropertyName.Rounded);
            Editor.DoPreSaveReset(this, SpinBox.PropertyName.CustomArrowRound);
            return;
        }
    }
}
#endif
