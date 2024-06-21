#if TOOLS
using Godot;
using Godot.Collections;

namespace F00F;

public partial class GUI
{
    [Export] private PackedScene MainMenu { get; set => this.Set(ref field, value, OnMainMenuSet); }

    private void OnMainMenuSet()
    {
        var mm = GetNodeOrNull("MainMenu");
        if (mm is not null) this.RemoveChild(mm, free: true);

        mm = MainMenu?.New(mm => mm.Name = "MainMenu");
        if (mm is not null) this.AddChild(mm, own: true);
    }

    public sealed override void _ValidateProperty(Dictionary property)
    {
        if (Editor.SetDisplayOnly(property, PropertyName.MainMenu)) return;
    }

    public sealed override void _Notification(int what)
    {
        if (Editor.OnPreSave(what))
        {
            Editor.DoPreSaveReset(this, PropertyName.Layer, 1);
            return;
        }

        if (Editor.OnPostSave(what))
            Editor.DoPostSaveRestore();
    }
}
#endif
