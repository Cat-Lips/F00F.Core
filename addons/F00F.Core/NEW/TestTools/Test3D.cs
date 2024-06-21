using System.Collections.Generic;
using Godot;
using static Godot.Performance;

namespace F00F.NEW;

[Tool]
public partial class Test3D : Game3D
{
    #region Private

    protected Node World => field ??= GetNode("World");
    protected Options Options => field ??= (Options)UX.GetNode("Options");
    protected Settings Settings => field ??= (Settings)UX.GetNode("Settings");
    private ResourcePreloader Preload => field ??= (ResourcePreloader)GetNode("Preload");

    #endregion

    #region Export

    protected enum TestWorldType { None, Arena, Terrain }
    [Export] protected TestWorldType WorldType { get; set => this.Set(ref field, value, InitWorld); } = TestWorldType.Arena;
    [Export] protected bool ShowSettingsWithMouse { get; set => this.Set(ref field, value, ShowSettings); } = true;

    #endregion

    protected virtual void InitOptions() { }
    protected virtual void InitSettings() { }
    protected virtual void InitWorld(Vector3 spawn)
    {
        Camera.LookAtFromPosition(
            spawn + Camera.Config.TargetFollowOffset * 2,
            spawn + Camera.Config.TargetLookAtOffset * 2);
    }

    #region Godot

    protected sealed override void OnEditorReady()
        => InitWorld();

    protected sealed override void OnReady()
    {
        LoadCfg();
        InitWorld();
        InitOptions();
        InitSettings();
        ShowSettings();

        void LoadCfg()
            => WorldType = LoadEnum(WorldType);

        void InitOptions()
        {
            AddWorldOptions();
            this.InitOptions();
            AddDebugOptions();

            void AddWorldOptions()
            {
                const string CameraState = "Camera";
                const string TerrainHeight = " - Terrain Height";
                const string TargetPosition = " - Target Position";
                const string TargetAltitude = " - Target Altitude";
                TestTerrain.State TerrainData = default;

                var uiWorldEdit = UI.OpenButton(OnWorldEdit);
                var uiWorldLabel = UI.Label(nameof(WorldType));
                var uiWorldSelect = UI.EnumEdit(OnWorldSelect, WorldType);
                var uiWorld = UI.Layout("World", uiWorldSelect, uiWorldEdit, uiWorldLabel);
                MyInput.ShowWithMouse(uiWorldSelect, uiWorldEdit); MyInput.HideWithMouse(uiWorldLabel);

                Options.Sep();
                Options.Add("World", uiWorld);
                Options.Add(CameraState, GetCameraState, urgent: true);
                Options.Add(TerrainHeight, GetTerrainHeight, urgent: true);
                Options.Add(TargetPosition, GetTargetPosition, urgent: true);
                Options.Add(TargetAltitude, GetTargetAltitude, urgent: true);

                OnWorldSelect(WorldType);

                string GetCameraState()
                {
                    return $"{CamPos()} [{CamMode()}]";

                    string CamPos()
                        => $"{Camera.GlobalPosition.Rounded()}";

                    string CamMode()
                    {
                        return string.Join(" ", Parts());

                        IEnumerable<string> Parts()
                        {
                            if (Camera.Target.NotNull())
                                yield return Camera.Target.Name;
                            yield return $"{Camera.Mode}";
                        }
                    }
                }

                string GetTerrainHeight()
                {
                    var target = Camera.Target ?? Camera;
                    TerrainData = Terrain.GetState(target.GlobalPosition);
                    return $"{TerrainData.Height.Rounded(1)}";
                }

                string GetTargetPosition()
                    => $"{TerrainData.Position.Rounded(1)}";

                string GetTargetAltitude()
                    => $"{TerrainData.Altitude.Rounded(1)}";

                void OnWorldEdit()
                {
                    switch (WorldType)
                    {
                        case TestWorldType.None:
                            Settings.RemoveGroup("Arena");
                            Settings.RemoveGroup("Terain");
                            break;
                        case TestWorldType.Arena:
                            Settings.ToggleGroup("Arena", Arena.Config, x => Arena.Config = x);
                            Settings.RemoveGroup("Terain");
                            break;
                        case TestWorldType.Terrain:
                            Settings.RemoveGroup("Arena");
                            Settings.ToggleGroup("Terrain", Terrain.Config, x => Terrain.Config = x);
                            break;
                    }
                }

                void OnWorldSelect(TestWorldType x)
                {
                    Settings.RemoveGroup("Arena");
                    Settings.RemoveGroup("Terrain");

                    uiWorldLabel.Text = $"{WorldType = x}";
                    var IsTerrain = x is TestWorldType.Terrain;

                    Options.Show(TerrainHeight, IsTerrain);
                    Options.Show(TargetPosition, IsTerrain);
                    Options.Show(TargetAltitude, IsTerrain);
                }
            }

            void AddDebugOptions()
            {
                Options.Sep();
                InitDebugDraw();
                AddDebugDrawOptions();
                AddGodotDebugOptions();
                AddGodotPerformanceMonitors();

                void InitDebugDraw()
                    => DebugDraw.Enabled = GetTree().DebugCollisionsHint;

                void AddDebugDrawOptions()
                    => Options.Add("DebugDraw", UI.Toggle("DebugDraw", DebugDraw.Enabled, on => DebugDraw.Enabled = on));

                void AddGodotDebugOptions()
                    => Options.Add("DebugDraw", UI.EnumEdit("DebugDraw", GetViewport().DebugDraw, x => GetViewport().DebugDraw = x));

                void AddGodotPerformanceMonitors()
                {
                    var uiPerfAdd = UI.AddButton(OnPerfAdd);
                    var uiPerfLabel = UI.Label("PerfType");
                    var uiPerfSelect = UI.EnumEdit<Monitor>(OnPerfSelect);
                    var uiPerf = UI.Layout("Perf", uiPerfSelect, uiPerfAdd, uiPerfLabel);
                    MyInput.ShowWithMouse(uiPerfSelect, uiPerfAdd); MyInput.HideWithMouse(uiPerfLabel);

                    void OnPerfAdd()
                    {

                    }

                    void OnPerfSelect(Monitor perf)
                    {

                    }
                }
            }
        }
    }

    #endregion

    #region Private

    protected TestArena Arena { get; private set; }
    protected TestTerrain Terrain { get; private set; }

    private void InitWorld()
    {
        if (this.IsReady())
        {
            DestroyWorld();
            CreateWorld();
            SaveWorld();
        }

        void DestroyWorld()
        {
            Arena = null; Terrain = null;
            World.RemoveChildren(x => x.Owner is null);
        }

        void CreateWorld()
        {
            switch (WorldType)
            {
                case TestWorldType.None:
                    InitWorld(Vector3.Zero);
                    break;
                case TestWorldType.Arena:
                    Arena = CreateWorld<TestArena>();
                    break;
                case TestWorldType.Terrain:
                    Terrain = CreateWorld<TestTerrain>();
                    break;
            }

            T CreateWorld<T>() where T : Node, ISpawnReady
            {
                var world = Preload.New<T>();
                World.AddChild(world);
                if (world.SpawnPoint.HasValue)
                    InitWorld(world.SpawnPoint.Value);
                world.SpawnReady += InitWorld;
                return world;
            }
        }

        void SaveWorld()
        {
            if (Editor.IsEditor) return;
            SaveEnum(WorldType);
        }
    }

    private void ShowSettings()
    {
        if (ShowSettingsWithMouse)
        {
            ShowSettings();
            Camera.SelectModeChanged -= ShowSettings;
            Camera.SelectModeChanged += ShowSettings;
        }
        else
        {
            Settings.Activate();
            Camera.SelectModeChanged -= ShowSettings;
        }

        void ShowSettings()
            => Settings.Activate(Camera.SelectMode);
    }

    #endregion
}
