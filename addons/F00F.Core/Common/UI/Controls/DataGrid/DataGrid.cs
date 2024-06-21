using System.Linq;
using Godot;

namespace F00F;

[Tool]
public partial class DataGrid : GridContainer
{
    private const int _SortChildren = (int)NotificationSortChildren;

    public sealed override void _Notification(int what)
    {
        if (what is _SortChildren)
        {
            var size = GetMinimumSize();
            CustomMinimumSize = size;
        }

        Vector2 GetMinimumSize()
        {
            return new(MaxLabelWidth(), 0);

            float MaxLabelWidth()
                => this.GetChildren<Label>()
                    .Where(x => x.Name.EndsWith("Label"))
                    .MaxOrDefault(x => x.Size.X) * Columns;
        }
#if TOOLS
        if (Editor.OnPreSave(what))
        {
            Editor.DoPreSaveReset(this, PropertyName.CustomMinimumSize);
            return;
        }

        if (Editor.OnPostSave(what))
            Editor.DoPostSaveRestore();
#endif
    }
}
