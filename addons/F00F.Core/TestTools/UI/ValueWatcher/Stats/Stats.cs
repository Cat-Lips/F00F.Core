using System;
using System.Diagnostics;
using Godot;

namespace F00F;

using RenderingInfo = RenderingServer.RenderingInfo;

[Tool]
public partial class Stats : ValueWatcher
{
    protected sealed override void OnReady()
    {
        AddFPS();
        AddMEM();
        AddVRAM();
        AddDrawCalls();
        AddFrameTime();
    }

    [Conditional("DEBUG")]
    private void AddMEM() => Add("MEM", () => { GC.Collect(); return Utils.HumaniseBytes(OS.GetStaticMemoryUsage()); });
    private void AddFPS() => Add("FPS", Engine.GetFramesPerSecond);
    private void AddVRAM() => Add("VRAM", () => Utils.HumaniseBytes(RenderingServer.GetRenderingInfo(RenderingInfo.VideoMemUsed)));
    private void AddDrawCalls() => Add("DrawCalls", () => RenderingServer.GetRenderingInfo(RenderingInfo.TotalDrawCallsInFrame));
    private void AddFrameTime() => Add("FrameTime", () => GetProcessDeltaTime().Rounded(5));
}
