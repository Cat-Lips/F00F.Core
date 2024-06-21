#if TOOLS
using Godot.Collections;

namespace F00F.Core;

public partial class PartConfig
{
    public override void _ValidateProperty(Dictionary property)
    {
        if (Editor.SetReadOnly(property, PropertyName.Path)) return;
        if (Editor.SetDisplayOnly(property, PropertyName.Visible)) return;
        if (Editor.Show(property, PropertyName.MaxShapes, Shape is GlbShapeType.MultiConvex)) return;
        if (Editor.Hide(property, PropertyName.Disabled, Shape is GlbShapeType.None)) return;
    }
}
#endif
