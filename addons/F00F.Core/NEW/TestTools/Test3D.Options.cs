using System;
using System.Linq;
using Godot;

namespace F00F.NEW;

using static DisplayServer;
using static Performance;

public partial class Test3D
{
    private void AddWorldOptions()
    {
        const string TargetPosition = " - Position";
        const string TargetAltitude = " - Altitude";
        const string TargetHeight = " - Height";
        TestTerrain.State TerrainState = default;

        var uiWorldEdit = UI.OpenButton(OnWorldEdit);
        var uiWorldSelect = UI.EnumEdit(OnWorldSelect, WorldType);
        var uiWorldConfig = UI.Layout("WorldConfig", uiWorldSelect, uiWorldEdit);

        Options.Add("World", uiWorldConfig);
        Options.Add(TargetPosition, GetTargetPosition, urgent: true);
        Options.Add(TargetAltitude, GetTargetAltitude, urgent: true);
        Options.Add(TargetHeight, GetTargetHeight, urgent: true);

        OnWorldSelect(WorldType);

        string GetTargetPosition()
        {
            TerrainState = GetTerrainState();
            return $"{TerrainState.Position.Rounded(1)}";

            TestTerrain.State GetTerrainState()
            {
                var target = Camera.Target ?? Camera;
                return terrain.GetState(target.GlobalPosition);
            }
        }

        string GetTargetAltitude()
            => $"{TerrainState.Altitude.Rounded(1)}";

        string GetTargetHeight()
            => $"{TerrainState.Height.Rounded(1)}";

        void OnWorldEdit()
        {
            switch (WorldType)
            {
                case TestWorldType.None:
                    Settings.RemoveGroup("Arena");
                    Settings.RemoveGroup("Terain");
                    break;
                case TestWorldType.Arena:
                    Settings.ToggleGroup("Arena", arena.Config, x => arena.Config = x);
                    Settings.RemoveGroup("Terain");
                    break;
                case TestWorldType.Terrain:
                    Settings.RemoveGroup("Arena");
                    Settings.ToggleGroup("Terrain", terrain.Config, x => terrain.Config = x);
                    break;
            }
        }

        void OnWorldSelect(TestWorldType x)
        {
            Settings.RemoveGroup("Arena");
            Settings.RemoveGroup("Terrain");

            var IsTerrain = x is TestWorldType.Terrain;

            Options.Show(TargetPosition, IsTerrain);
            Options.Show(TargetAltitude, IsTerrain);
            Options.Show(TargetHeight, IsTerrain);

            WorldType = x;
        }
    }

    private void AddDebugOptions()
    {
        const string Title = "DebugDraw";
        DebugDraw.Enabled = GetTree().DebugCollisionsHint;
#if DEBUG_DRAW
        var ddToggle = UI.Toggle(Title, DebugDraw.Enabled, on => DebugDraw.Enabled = on);
        var ddSelect = UI.EnumEdit(Title, GetViewport().DebugDraw, x => GetViewport().DebugDraw = x);
        Options.Add(Title, UI.Layout(Title, ddSelect, ddToggle));
#else
        var ddSelect = UI.EnumEdit(Title, GetViewport().DebugDraw, x => GetViewport().DebugDraw = x);
        Options.Add(Title, ddSelect);
#endif
    }

    private void AddPerfMetrics()
    {
        const string Title = "Performance";
        EnumOptionButton<Monitor> PerfSelect = null;

        InitVSync();
        InitPerfs();
        LoadPerfs();

        void InitVSync()
        {
            var vSync = UI.EnumEdit(nameof(VSyncMode), IX.VSyncMode, x => IX.VSyncMode = x);
            IX.VSyncModeChanged += () => vSync.Selected = IX.VSyncMode;
            Options.Add(vSync);

            Options.VisibilityChanged += () => { if (!Options.Visible) IX.ResetVSyncMode(); };
        }

        void InitPerfs()
        {
            var uiPerfSelect = PerfSelect = UI.EnumSelect<Monitor>("Perfs");
            var uiPerfAdd = UI.AddButton("Perfs", () => AddPerf(uiPerfSelect.Selected));
            var uiPerf = UI.Layout("Perfs", uiPerfSelect, uiPerfAdd);
            Options.Add(Title, uiPerf);
        }

        void LoadPerfs()
            => LoadEnums<Monitor>(Title)?.ForEach(AddPerf);

        void SavePerfs()
            => SaveEnums(Title, PerfSelect.GetDisabledItems().ToArray());

        void AddPerf(Monitor perf)
        {
            var key = $" - {perf.Str()}";
            var uiPerfRemove = UI.RemoveButton(OnPerfRemove);
            Options.Add(key, PerfFormatFunc(), uiPerfRemove, PerfUrgency());
            PerfSelect.DisableItem(perf);
            SavePerfs();

            void OnPerfRemove()
            {
                PerfSelect.EnableItem(perf);
                Options.Remove(key);
                SavePerfs();
            }

            bool PerfUrgency()
                => false;

            Func<string> PerfFormatFunc()
            {
                return perf switch
                {
                    Monitor.TimeProcess or
                    Monitor.TimePhysicsProcess or
                    Monitor.TimeNavigationProcess or

                    Monitor.AudioOutputLatency => TimeStr,

                    Monitor.MemoryStatic or // Debug only
                    Monitor.MemoryStaticMax or // Debug only
                    Monitor.MemoryMessageBufferMax or

                    Monitor.RenderVideoMemUsed or
                    Monitor.RenderBufferMemUsed or
                    Monitor.RenderTextureMemUsed or
                    Monitor.RenderStreamingTextureMemUsed => ByteStr,

                    _ => RawStr,
                };

                string RawStr()
                    => $"{GetMonitor(perf)}";

                string ByteStr()
                    => $"{Utils.HumaniseBytes((ulong)GetMonitor(perf))}";

                string TimeStr()
                    => $"{Utils.HumaniseTime(TimeSpan.FromSeconds(GetMonitor(perf)))}";
            }
        }
    }
}
