using System;
using System.Collections.Generic;
using Godot;

namespace F00F;

using ControlPair = (Control Label, Control EditControl);

public partial class PlayerConfig : IEditable<PlayerConfig>
{
    public virtual IEnumerable<ControlPair> GetEditControls() => GetEditControls(out var _);
    public IEnumerable<ControlPair> GetEditControls(out Action<PlayerConfig> SetData)
    {
        var ec = EditControls(out SetData);
        SetData(this);
        return ec;
    }

    public static IEnumerable<ControlPair> EditControls(out Action<PlayerConfig> SetData)
    {
        return UI.Create(out SetData, CreateUI);

        static void CreateUI(UI.IBuilder ui)
        {
            ui.AddValue(nameof(JumpStrength));
            ui.AddValue(nameof(MovementSpeed));
        }
    }
}
