using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace F00F;

using UX = UI;

[Tool]
public partial class Test3D : Game3D
{
    protected Options Options => field ??= GetNode<Options>("%Options");
    protected Settings Settings => field ??= GetNode<Settings>("%Settings");

    protected sealed override IGameConfig GameConfig => field ??= new TestConfig();

    protected virtual void InitOptions() { }
    protected virtual void InitSettings() { }
    protected virtual void InitSettings(out bool ShowSettingsOnCameraSelect)
    {
        ShowSettingsOnCameraSelect = true;
        InitSettings();
    }

    protected sealed override void OnReady()
    {
        InitDebug();
        InitOptions();
        InitSettings(out var show);
        if (show) ShowSettings();

        void InitDebug()
        {
            AddCamOptions();
            AddDebugOptions();

            void AddCamOptions()
            {
                //var terrain = GetNodeOrNull<Terrain>("Terrain");

                Options.Sep();
                Options.Add("Camera", () => $"{CamPos()} [{CamMode()}]", urgent: true);

                string CamPos() => $"{Camera.GlobalPosition.Rounded()}";

                //string CamPos() => terrain.IsNull()
                //    ? $"{Camera.GlobalPosition.Rounded()}"
                //    : $"{Camera.GlobalPosition.XZ().Rounded()} ({terrain.AltitudeDbg(Camera.GlobalPosition)}m)";

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

            void AddDebugOptions()
            {
                Options.Sep();
                DebugDraw.Enabled = GetTree().DebugCollisionsHint;
                AddDebugDrawOptions();
                AddDebugOptions();

                [Conditional("DEBUG_DRAW_3D")]
                static void AddDebugDrawOptions()
                    => ValueWatcher.Instance.Add("DebugDraw", UX.Toggle("DebugDraw", DebugDraw.Enabled, on => DebugDraw.Enabled = on));

                void AddDebugOptions()
                    => Options.Add("DebugDraw", UX.EnumEdit("DebugDraw", GetViewport().DebugDraw, x => GetViewport().DebugDraw = x));
            }
        }
    }

    private void ShowSettings()
    {
        Settings.Show(Camera.SelectMode);
        Camera.SelectModeSet += () => Settings.Show(Camera.SelectMode);
    }
}
