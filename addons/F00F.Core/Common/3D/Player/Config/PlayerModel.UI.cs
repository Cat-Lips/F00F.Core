using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace F00F;

using ControlPair = (Control Label, Control EditControl);

public partial class PlayerModel : IEditable<PlayerModel>
{
    public override IEnumerable<ControlPair> GetEditControls() => GetEditControls(out var _);
    public IEnumerable<ControlPair> GetEditControls(out Action<PlayerModel> SetData)
    {
        var ec = EditControls(out SetData);
        SetData(this);
        return ec;
    }

    public static IEnumerable<ControlPair> EditControls(out Action<PlayerModel> SetData)
    {
        var baseControls = ModelConfig.EditControls(out var SetBaseData);
        var myControls = UI.Create<PlayerModel>(out var SetMyData, CreateUI);
        SetData = x => { SetBaseData(x); SetMyData(x); };
        return baseControls.Concat(myControls);

        static void CreateUI(UI.IBuilder ui)
        {
            ui.AddOption(nameof(BoundingShape), items: UI.Items<GlbSimpleShapeType>());
            ui.AddOption(nameof(PartsShape), items: UI.Items<GlbShapeType>());
        }
    }
}
