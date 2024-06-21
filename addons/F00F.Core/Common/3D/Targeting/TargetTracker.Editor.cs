#if TOOLS
using Godot.Collections;

namespace F00F;

public partial class TargetTracker
{
    public sealed override void _ValidateProperty(Dictionary property)
    {
        if (Editor.Show(property, PropertyName.OffScreenMargin, ShowOffScreen)) return;
        if (Editor.Show(property, PropertyName.SlowHighlight, ShowVelocityHighlights)) return;
        if (Editor.Show(property, PropertyName.FastHighlight, ShowVelocityHighlights)) return;
        if (Editor.Show(property, PropertyName.SlowThreshold, ShowVelocityHighlights)) return;
        if (Editor.Show(property, PropertyName.FastThreshold, ShowVelocityHighlights)) return;
    }
}
#endif
