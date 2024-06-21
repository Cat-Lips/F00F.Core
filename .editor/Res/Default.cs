// meta-default: true
using Godot;

namespace F00F;

using static FastNoiseLite;

[Tool, GlobalClass]
public partial class _CLASS_ : CustomResource
{
    #region Enums

    public static class Enum
    {
        
    }

    #endregion

    #region Defaults

    private static class Default
    {
        //public static int XXX => 1024;
    }

    #endregion

    #region Exports

    //[ExportGroup("Detail")]
    //[Export] public int XXX { get; set => this.Set(ref field, value.ToPo2(field)); } = Default.XXX;

    #endregion

    #region Private

    public OceanConfig()
    {
        DisableChangedEvent();
        //YYY = Default.YYY; // Resource must be initialised here to track changed state
        EnableChangedEvent();
    }

    #endregion
}

//
// _CLASS_.UI.cs
//

using ControlPair = (Control Label, Control EditControl);

public partial class _CLASS_ : IEditable<_CLASS_>
{
    public virtual IEnumerable<ControlPair> GetEditControls() => GetEditControls(out var _);
    public IEnumerable<ControlPair> GetEditControls(out Action<_CLASS_> SetData)
    {
        var ec = EditControls(out SetData);
        SetData(this);
        return ec;
    }

    public static IEnumerable<ControlPair> EditControls(out Action<_CLASS_> SetData)
    {
        return UI.Create(out SetData, CreateUI);

        static void CreateUI(UI.IBuilder ui)
        {
            //ui.AddValue(nameof(FloorSize), @int: true, range: (0, null, null));
            //ui.AddOption(nameof(FloorShape), items: UI.Items<ShapeType>());
            //ui.AddCheck(nameof(ShowStunts));
            //ui.AddValue(nameof(StuntsScale), range: (1, null, null));
        }
    }
}