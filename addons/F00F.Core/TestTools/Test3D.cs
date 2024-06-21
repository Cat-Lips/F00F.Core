using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace F00F;

using UX = UI;

[Tool]
public partial class Test3D : Game3D
{
    protected Node World => field ??= GetNode("World");
    protected Options Options => field ??= (Options)GetNode("%Options");
    protected Settings Settings => field ??= (Settings)GetNode("%Settings");

    protected sealed override IGameConfig GameConfig => field ??= new TestConfig();

    protected enum TestWorldType { None, Arena, Terrain }
    [Export] protected TestWorldType WorldType { get; set => this.Set(ref field, value, InitWorld); }
    [Export] protected bool ShowSettingsWithMouse { get; set => this.Set(ref field, value, ShowSettings); } = true;

    protected virtual void InitOptions() { }
    protected virtual void InitSettings() { }
    protected virtual void InitWorld(Vector3 spawn)
    {
        Camera.LookAtFromPosition(
            spawn + Camera.Config.Offset * 2,
            spawn + Camera.Config.LookAt * 2);
    }

    protected sealed override void OnReady()
    {
        InitWorld();
        InitOptions();
        InitSettings();
        ShowSettings();

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

                var uxWorldEdit = UX.OpenButton(OnWorldEdit);
                var uxWorldLabel = UX.Label(nameof(WorldType));
                var uxWorldSelect = UX.EnumEdit(OnWorldSelect, WorldType);
                var uxWorld = UX.Layout("World", uxWorldSelect, uxWorldEdit, uxWorldLabel);
                MyInput.ShowWithMouse(uxWorldSelect, uxWorldEdit); MyInput.HideWithMouse(uxWorldLabel);

                Options.Sep();
                Options.Add("World", uxWorld);
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
                            yield return $"{Camera.CameraMode}";
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

                    uxWorldLabel.Text = $"{WorldType = x}";
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

                void InitDebugDraw()
                    => DebugDraw.Enabled = GetTree().DebugCollisionsHint;

                [Conditional("DEBUG_DRAW")]
                static void AddDebugDrawOptions()
                    => ValueWatcher.Instance.Add("DebugDraw", UX.Toggle("DebugDraw", DebugDraw.Enabled, on => DebugDraw.Enabled = on));

                void AddGodotDebugOptions()
                    => ValueWatcher.Instance.Add("DebugDraw", UX.EnumEdit("DebugDraw", GetViewport().DebugDraw, x => GetViewport().DebugDraw = x));
            }
        }
    }

    #region Private

    protected TestArena Arena { get; private set; }
    protected TestTerrain Terrain { get; private set; }
    private InstancePlaceholder ArenaPlaceholder => field ??= (InstancePlaceholder)World.GetNode("Arena");
    private InstancePlaceholder TerrainPlaceholder => field ??= (InstancePlaceholder)World.GetNode("Terrain");

    private void InitWorld()
    {
        if (this.NotReady()) return;
        DestroyWorld();
        CreateWorld();

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
                    Arena = (TestArena)ArenaPlaceholder.CreateInstance();
                    if (Arena.SpawnPoint.HasValue)
                        InitWorld(Arena.SpawnPoint.Value);
                    Arena.SpawnReady += InitWorld;
                    Arena.Visible = true;
                    break;
                case TestWorldType.Terrain:
                    Terrain = (TestTerrain)TerrainPlaceholder.CreateInstance();
                    if (Terrain.SpawnPoint.HasValue)
                        InitWorld(Terrain.SpawnPoint.Value);
                    Terrain.SpawnReady += InitWorld;
                    Terrain.Visible = true;
                    break;
            }
        }
    }

    private void ShowSettings()
    {
        if (ShowSettingsWithMouse)
        {
            ShowSettings();
            Camera.SelectModeSet -= ShowSettings;
            Camera.SelectModeSet += ShowSettings;
        }
        else
        {
            Settings.Show();
            Camera.SelectModeSet -= ShowSettings;
        }

        void ShowSettings()
            => Settings.Show(Camera.SelectMode);

    }
    #endregion
}
