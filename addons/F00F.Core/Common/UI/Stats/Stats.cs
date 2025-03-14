using System;
using System.Diagnostics;
using Godot;

namespace F00F;

[Tool]
public partial class Stats : ValueWatcher
{
    protected sealed override void OnReady1()
    {
        AddFPS();
        AddMEM();
        AddPOS();
    }

    [Conditional("DEBUG")]
    private void AddMEM() => Add("MEM", () => { GC.Collect(); return Utils.HumaniseBytes(OS.GetStaticMemoryUsage()); });
    private void AddPOS() => Add("POS", () => GetViewport().GetCamera3D()?.Position.Round(0));
    private void AddFPS() => Add("FPS", () => (int)Engine.GetFramesPerSecond());
}
