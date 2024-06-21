using System;
using System.Collections.Generic;
using Godot;

namespace F00F;

using static TestTerrainConfig.Enum;
using ControlPair = (Control Label, Control EditControl);

public partial class TestTerrainConfig : IEditable<TestTerrainConfig>
{
    public virtual IEnumerable<ControlPair> GetEditControls() => GetEditControls(out var _);
    public IEnumerable<ControlPair> GetEditControls(out Action<TestTerrainConfig> SetData)
    {
        var ec = EditControls(out SetData);
        SetData(this);
        return ec;
    }

    public static IEnumerable<ControlPair> EditControls(out Action<TestTerrainConfig> SetData)
    {
        return UI.Create(out SetData, CreateUI);

        static void CreateUI(UI.IBuilder ui)
        {
            //ui.AddTexture(nameof(HeightMap), nullable: false); // FIXME
            //ui.AddGradient(nameof(ColorMap), items: UI.Items<GlbRotate>()); // TODO
            //ui.AddCurve(nameof(Gradient));
            ui.AddOption(nameof(ShapeType), items: UI.Items<ShapeType>());
            ui.AddOption(nameof(MeshType), items: UI.Items<MeshType>());
            //ui.AddValue(nameof(Amplitude), range: (0, null, null));
        }
    }
}
