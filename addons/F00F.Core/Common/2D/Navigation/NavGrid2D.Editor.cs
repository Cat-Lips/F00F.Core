#if TOOLS
using Godot;
using Godot.Collections;

namespace F00F;

public partial class NavGrid2D
{
    public sealed override void _ValidateProperty(Dictionary property)
    {
        if (Editor.Show(property, PropertyName.TileMap, GetParentOrNull<TileMapLayer>() is null)) return;
    }
}
#endif
