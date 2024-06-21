using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace F00F;

using UX = UI;

[Tool]
public partial class Test3D : Game3D
{
    protected Node World => field ??= GetNode<Node>("World");
    protected Options Options => field ??= GetNode<Options>("%Options");
    protected Settings Settings => field ??= GetNode<Settings>("%Settings");

    protected virtual bool AlwaysShowSettings => false;
    protected sealed override IGameConfig GameConfig => field ??= new TestConfig();

    protected enum TestWorldType { None, Arena, Terrain }
    [Export] protected TestWorldType WorldType { get; set => this.Set(ref field, value, InitWorld); }

    protected virtual void InitOptions() { }
    protected virtual void InitSettings() { }
    protected virtual void InitWorld(Vector3 spawn)
        => Camera.GlobalPosition = spawn;

    protected sealed override void OnReady()
    {
        InitWorld();
        InitDebug();
        InitOptions();
        InitSettings();
        ShowSettings();

        void InitDebug()
        {
            AddWorldOptions();
            AddDebugOptions();

            void AddWorldOptions()
            {
                const string CameraState = "Camera";
                const string TerrainHeight = " - Terrain Height";
                const string TargetPosition = " - Target Position";
                const string TargetAltitude = " - Target Altitude";
                TestTerrain.State TerrainData = default;

                Options.Sep();
                Options.Add(UX.EnumEdit(WorldType, InitWorld));
                Options.Add(CameraState, GetCameraState, urgent: true);
                Options.Add(TerrainHeight, GetTerrainHeight, urgent: true);
                Options.Add(TargetPosition, GetTargetPosition, urgent: true);
                Options.Add(TargetAltitude, GetTargetAltitude, urgent: true);

                InitWorld(WorldType);

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

                void InitWorld(TestWorldType x)
                {
                    if ((WorldType = x) is TestWorldType.Terrain)
                    {
                        Options.Show(TerrainHeight);
                        Options.Show(TargetPosition);
                        Options.Show(TargetAltitude);
                    }
                    else
                    {
                        Options.Hide(TerrainHeight);
                        Options.Hide(TargetPosition);
                        Options.Hide(TargetAltitude);
                    }
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

                [Conditional("DEBUG_DRAW_3D")]
                static void AddDebugDrawOptions()
                    => ValueWatcher.Instance.Add("DebugDraw", UX.Toggle("DebugDraw", DebugDraw.Enabled, on => DebugDraw.Enabled = on));

                void AddGodotDebugOptions()
                    => ValueWatcher.Instance.Add("DebugDraw", UX.EnumEdit("DebugDraw", GetViewport().DebugDraw, x => GetViewport().DebugDraw = x));
            }
        }

        void ShowSettings()
        {
            if (AlwaysShowSettings) return;
            Settings.Show(Camera.SelectMode);
            Camera.SelectModeSet += () => Settings.Show(Camera.SelectMode);
        }
    }

    #region Private

    private TestArena Arena { get; set; }
    private TestTerrain Terrain { get; set; }
    private InstancePlaceholder ArenaPlaceholder => field ??= World.GetNode<InstancePlaceholder>("Arena");
    private InstancePlaceholder TerrainPlaceholder => field ??= World.GetNode<InstancePlaceholder>("Terrain");

    private void InitWorld()
    {
        if (this.NotReady()) return;
        DestroyWorld();
        CreateWorld();

        void DestroyWorld()
        {
            Arena = null; Terrain = null;
            World.RemoveChildren(x => x != ArenaPlaceholder && x != TerrainPlaceholder);
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
                    InitWorld(Arena.SpawnPoint.Position);
                    Arena.Visible = true;
                    break;
                case TestWorldType.Terrain:
                    Terrain = (TestTerrain)TerrainPlaceholder.CreateInstance();
                    Terrain.SpawnReady += InitWorld;
                    Terrain.Visible = true;
                    break;
            }
        }
    }

    #endregion
}
