#if TOOLS
using Godot.Collections;

namespace F00F.Core;

public partial class PhysicsConfig
{
    public override void _ValidateProperty(Dictionary property)
    {
        if (Editor.Hide(property, PropertyName.Parts, Parts.Length is 0)) return;
    }
}
#endif
