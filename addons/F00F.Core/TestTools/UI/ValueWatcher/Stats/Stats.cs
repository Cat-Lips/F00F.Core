using System;
using System.Diagnostics;
using Godot;
using static Godot.RenderingServer;

namespace F00F;

[Tool]
public partial class Stats : ValueWatcher
{
    public float FPS { get; private set; }
    public ulong MEM { get; private set; }
    public ulong VRAM { get; private set; }
    public ulong DrawCalls { get; private set; }
    public float FrameTime { get; private set; }

    [Export(PropertyHint.Range, "1,5")] public int Rounding { get; set; } = 3;
    [Export] public bool ShowMore { get; set => this.Set(ref field, value, () => { Update(true); ResetWidth(); }); }

    public float MinFPS { get; private set; }
    public ulong MaxMEM { get; private set; }
    public ulong MaxVRAM { get; private set; }
    public ulong MaxDrawCalls { get; private set; }
    public float MaxFrameTime { get; private set; }

    protected sealed override void Initialise()
    {
        AddFPS();
        AddVRAM();
        AddMEM(this);
        AddDrawCalls();
        AddFrameTime();

        void AddFPS() => Add("FPS", GetFPS, urgent: true);
        void AddVRAM() => Add("VRAM", GetVRAM, urgent: true);
        void AddDrawCalls() => Add("DrawCalls", GetDrawCalls, urgent: true);
        void AddFrameTime() => Add("FrameTime", GetFrameTime, urgent: true);

        [Conditional("DEBUG")]
        static void AddMEM(Stats self) => self.Add("MEM", self.GetMEM, urgent: false);
    }

    private string GetFPS() { MinFPS = Math.Min(MinFPS, FPS = (float)Engine.GetFramesPerSecond()); return ShowMore ? $"{FPS} [Min: {MinFPS}]" : $"{FPS}"; }
    private string GetVRAM() { MaxVRAM = Math.Max(MaxVRAM, VRAM = GetRenderingInfo(RenderingInfo.VideoMemUsed)); return ShowMore ? $"{Utils.HumaniseBytes(VRAM)} [Max: {Utils.HumaniseBytes(MaxVRAM)}]" : $"{Utils.HumaniseBytes(VRAM)}"; }
    private string GetDrawCalls() { MaxDrawCalls = Math.Max(MaxDrawCalls, DrawCalls = GetRenderingInfo(RenderingInfo.TotalDrawCallsInFrame)); return ShowMore ? $"{DrawCalls} [Max: {MaxDrawCalls}]" : $"{DrawCalls}"; }
    private string GetFrameTime() { MaxFrameTime = Math.Max(MaxFrameTime, FrameTime = (float)GetProcessDeltaTime()); return ShowMore ? $"{FrameTime.Rounded(Rounding)} [Max: {MaxFrameTime.Rounded(Rounding)}]" : $"{FrameTime.Rounded(Rounding)}"; }
    private string GetMEM() { GC.Collect(); MaxMEM = Math.Max(MaxMEM, MEM = OS.GetStaticMemoryUsage()); return ShowMore ? $"{Utils.HumaniseBytes(MEM)} [Max: {Utils.HumaniseBytes(MaxMEM)}]" : Utils.HumaniseBytes(MEM); }
}
