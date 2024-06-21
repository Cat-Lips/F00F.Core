using Godot;
using static Godot.DisplayServer;

namespace F00F.NEW;

[Tool]
public partial class Test3D : Game3D
{
    #region Private

    protected Node World => field ??= GetNode("World");
    protected Options Options => field ??= (Options)UX.GetNode("Options");
    protected Settings Settings => field ??= (Settings)UX.GetNode("Settings");
    protected ResourcePreloader Preload => field ??= (ResourcePreloader)GetNode("Preload");

    #endregion

    #region Export

    protected enum TestWorldType { None, Arena, Terrain }
    [Export] protected TestWorldType WorldType { get; set => this.Set(ref field, value, InitWorld); } = TestWorldType.Arena;

    #endregion

    protected virtual void InitOptions() { }
    protected virtual void InitSettings() { }
    protected virtual void InitWorld(in Vector3 spawn) { }
    protected virtual void InitCamera(in Vector3 spawn)
    {
        Camera.LookAtFromPosition(
            spawn + Camera.Config.TargetFollowOffset * 2,
            spawn + Camera.Config.TargetLookAtOffset * 2);
    }

    #region Godot

    protected sealed override void OnReady()
    {
        LoadCfg();
        InitWorld();
        InitOptions();
        InitSettings();

        void LoadCfg()
        {
            if (Editor.IsEditor) return;
            WorldType = LoadEnum(WorldType);
        }

        void InitOptions()
        {
            AddWorldOptions();
            this.InitOptions();
            AddDebugOptions();
            AddPerfMetrics();
        }
    }

    protected override bool OnUnhandledKeyInput(InputEventKey e)
    {
        return this.Handle(e.IsActionJustPressed(MyInput.ToggleVSync), ToggleVSync)
            || this.Handle(e.IsActionJustPressed(MyInput.ToggleValues), ToggleValues);

        void ToggleVSync()
            => IX.VSyncMode = Options.Visible && IX.VSyncMode is VSyncMode.Enabled ? VSyncMode.Disabled : VSyncMode.Enabled;

        void ToggleValues()
            => Settings.Activate(Options.Visible = !Options.Visible);
    }

    #endregion
}
