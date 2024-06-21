using System;
using Godot;

namespace F00F;

using static DisplayServer;

public partial class MyInput
{
    public event Action VSyncModeChanged;
    public VSyncMode VSyncMode { get; set => this.Set(ref field, value, () => WindowSetVsyncMode(VSyncMode), VSyncModeChanged); } = Default.VSyncMode;

    public void ResetVSyncMode() => VSyncMode = Default.VSyncMode;

    public static partial class Default
    {
        public static readonly VSyncMode VSyncMode = WindowGetVsyncMode();
    }
}
