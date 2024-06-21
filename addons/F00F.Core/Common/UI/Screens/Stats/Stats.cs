using System;
using System.Diagnostics;
using Godot;

namespace F00F;

[Tool]
public partial class Stats : ValueWatcher
{
    protected sealed override void OnReady()
    {
        AddFPS();
        AddMEM();
    }

    [Conditional("DEBUG")]
    private void AddMEM() => Add("MEM", () => { GC.Collect(); return Utils.HumaniseBytes(OS.GetStaticMemoryUsage()); });
    private void AddFPS() => Add("FPS", () => (int)Engine.GetFramesPerSecond());
}
