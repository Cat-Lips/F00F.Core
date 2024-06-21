using System.Diagnostics;

namespace F00F;

public partial class Camera
{
    [Conditional("DEBUG")]
    private void InitDebug()
    {
        ValueWatcher.Instance?.Sep("Camera", UI.NewOpenButton("Config", OnConfigEdit));
        ValueWatcher.Instance?.Add(" - Mode", ModeStr, urgent: true);
        ValueWatcher.Instance?.Add(" - Shake", () => Shake.Rounded(ValueWatcher.DefaultRound), urgent: true);
        ValueWatcher.Instance?.Add(" - Position", () => GlobalPosition.Rounded());

        string ModeStr() => Mode switch
        {
            CameraMode.Free => $"{Mode} ({_speed})",
            CameraMode.Select => $"{Mode}",
            CameraMode.Follow => $"{Mode}{(_orbitLock ? "*" : null)} ({Target.Name})",
            _ => null,
        };

        void OnConfigEdit()
            => Settings.Instance.ToggleGroup("Camera", Config, x => Config = x);
    }
}
