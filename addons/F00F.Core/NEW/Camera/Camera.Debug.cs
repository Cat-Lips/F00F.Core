using System.Diagnostics;
using Godot;

namespace F00F;

public partial class Camera
{
    [Conditional("DEBUG")]
    private void InitDebug()
    {
        if (Settings.Instance.IsNull()) return;
        if (ValueWatcher.Instance.IsNull()) return;

        var uiEditCameraConfig = UI.NewOpenButton("EditCameraConfig", OnEditCameraConfig);
        ValueWatcher.Instance.Add("Camera", CamStr, uiEditCameraConfig, urgent: true);
        TargetChanged += () =>
        {
            var cfgTarget = TryGetTargetConfig();

            if (cfgTarget.IsNull())
                Settings.Instance.Clear();
            else
                Settings.Instance.SetData(Target.Name, cfgTarget, TrySetTargetConfig);

            IEditable TryGetTargetConfig() => Target?.Get(PropertyName.Config).Obj as IEditable;
            void TrySetTargetConfig(IEditable cfg) => Target.Set(PropertyName.Config, cfg as Resource);
        };

        void OnEditCameraConfig()
            => Settings.Instance.ToggleGroup("Camera", Config, x => Config = x);

        string CamStr()
        {
            return $"{ShakeStr()}{PosStr()} {ModeStr()}";

            string ShakeStr() => Shake is 0 ? null : $"[Shake: {Shake.Rounded(ValueWatcher.DefaultRound)}] ";
            string SpeedStr() => _speed is 1 ? null : $" ({_speed})";
            string OrbitStr() => _orbitLock ? "*" : null;
            string PosStr() => $"{GlobalPosition.Rounded()}";
            string ModeStr() => Mode switch
            {
                CameraMode.Free => $"{Mode}{SpeedStr()}",
                CameraMode.Select => $"{Mode}",
                CameraMode.Follow => $"{Mode}{OrbitStr()} ({Target.Name})",
                _ => null,
            };
        }
    }
}
