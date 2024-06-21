using System;
using System.Collections.Generic;
using Godot;

namespace F00F;

using ControlPair = (Control Label, Control EditControl);

public partial class ModelConfig : IEditable<ModelConfig>
{
    public virtual IEnumerable<ControlPair> GetEditControls() => GetEditControls(out var _);
    public IEnumerable<ControlPair> GetEditControls(out Action<ModelConfig> SetData)
    {
        var ec = EditControls(out SetData);
        SetData(this);
        return ec;
    }

    public static IEnumerable<ControlPair> EditControls(out Action<ModelConfig> SetData)
    {
        return UI.Create(out SetData, CreateUI);

        static void CreateUI(UI.IBuilder ui)
        {
            ui.AddScene(nameof(Scene));
            ui.AddOption(nameof(Rotation), items: UI.Items<GlbRotate>());
            ui.AddValue(nameof(MassMultiplier), range: (0, null, null));
            ui.AddValue(nameof(ScaleMultiplier), range: (0, null, null));
            ui.AddValue(nameof(ShapeReductionRatio), range: (0, .5f, .01f));
        }
    }
}
