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
            ui.AddScene(nameof(Scene)); // TODO:  Only show if Source not set
            ui.AddOption(nameof(Rotation), items: UI.Items<GlbRotate>()); // TODO:  Only show if Source not set
            ui.AddValue(nameof(MassMultiplier), range: (0, null, null)); // TODO:  Only show if Source not set
            ui.AddValue(nameof(MeshScaleMultiplier), range: (0, null, null)); // TODO:  Only show if Source not set
            ui.AddValue(nameof(ShapeScaleMultiplier), range: (0, 1, null));
            ui.AddOption(nameof(PartsShape), items: UI.Items<GlbShapeType>());
            ui.AddOption(nameof(BoundingShape), items: UI.Items<GlbSimpleShapeType>());
        }
    }
}
