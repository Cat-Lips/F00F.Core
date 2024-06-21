using System;
using System.Diagnostics;
using Godot;

namespace F00F;

[Tool]
public partial class Stats : ValueWatcher
{
    protected virtual void OnReady2() { }
    protected sealed override void OnReady1()
    {
        AddFPS();
        AddMEM();
        OnReady2();
    }

    [Conditional("DEBUG")]
    private void AddMEM() => Add("MEM", () => { GC.Collect(); return Utils.HumaniseBytes(OS.GetStaticMemoryUsage()); });
    private void AddFPS() => Add("FPS", () => (int)Engine.GetFramesPerSecond());
}
