using System.Diagnostics;
using Godot;

namespace F00F;

using UX = UI;

[Tool]
public partial class Test3D : Game3D
{
    protected Options Options => field ??= GetNode<Options>("%Options");
    protected Settings Settings => field ??= GetNode<Settings>("%Settings");

    protected override IGameConfig GameConfig => field ??= new TestConfig();

    protected virtual void AddOptions() { }
    protected virtual void InitSettings() { }

    protected sealed override void OnReady()
    {
        InitDebug();
        AddOptions();

        InitSettings();
        ShowSettings();

        void InitDebug()
        {
            DebugDraw.Enabled = GetTree().DebugCollisionsHint;
            ValueWatcher.Instance.AddSep();
            AddDebugDrawOptions();
            AddDebugOptions();

            [Conditional("DEBUG_DRAW_3D")]
            static void AddDebugDrawOptions()
                => ValueWatcher.Instance.Add("DebugDraw", UX.Toggle("DebugDraw", DebugDraw.Enabled, on => DebugDraw.Enabled = on));

            void AddDebugOptions()
                => ValueWatcher.Instance.Add("DebugDraw", UX.EnumEdit("DebugDraw", GetViewport().DebugDraw, x => GetViewport().DebugDraw = x));
        }

        void ShowSettings()
        {
            Settings.Show(Camera.SelectMode);
            Camera.SelectModeSet += () => Settings.Show(Camera.SelectMode);
        }
    }
}
