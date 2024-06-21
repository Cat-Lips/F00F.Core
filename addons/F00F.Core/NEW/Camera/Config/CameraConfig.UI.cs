using System;
using System.Collections.Generic;
using Godot;

namespace F00F;

using static CameraConfig.Enum;
using ControlPair = (Control Label, Control EditControl);

public partial class CameraConfig : IEditable<CameraConfig>
{
    public virtual IEnumerable<ControlPair> GetEditControls() => GetEditControls(out var _);
    public IEnumerable<ControlPair> GetEditControls(out Action<CameraConfig> SetData)
    {
        var ec = EditControls(out SetData);
        SetData(this);
        return ec;
    }

    public static IEnumerable<ControlPair> EditControls(out Action<CameraConfig> SetData)
    {
        return UI.Create(out SetData, CreateUI);

        static void CreateUI(UI.IBuilder ui)
        {
            ui.AddGroup("FreeCam", "FreeCam", fold: true, enable: nameof(FreeCamEnabled));
            ui.AddValue(nameof(FreeCamMoveSpeed), range: (0, null, null));
            ui.AddValue(nameof(FreeCamSensitivity), range: (.001f, .009f, .001f));
            ui.AddCheck(nameof(FreeCamInvertMouseY));
            ui.AddValue(nameof(FreeCamMaxPitch), range: (Mathf.DegToRad(0), Mathf.DegToRad(90), null));
            ui.AddValue(nameof(FreeCamMinPitch), range: (Mathf.DegToRad(-90), Mathf.DegToRad(0), null));
            ui.EndGroup();
            ui.AddGroup("FollowCam", "Target", fold: true);
            ui.AddCheck(nameof(TargetMatchUp));
            ui.AddValue(nameof(TargetFollowSpeed), range: (0, null, null));
            ui.AddVec3(nameof(TargetFollowOffset));
            ui.AddVec3(nameof(TargetLookAtOffset));
            ui.AddGroup("Speed Zoom", "Zoom", fold: true, enable: nameof(ZoomEnabled));
            ui.AddValue(nameof(ZoomMaxSpeed), range: (0, null, null));
            ui.AddValue(nameof(ZoomMaxDistance), range: (0, null, null));
            ui.AddOption<ZoomMode>(nameof(ZoomMode));
            ui.EndGroup();
            ui.AddGroup("Look Behind", "LookBehind", fold: true, enable: nameof(LookBehindEnabled));
            ui.AddValue(nameof(LookBehindSpeed), range: (0, null, null));
            ui.EndGroup();
            ui.AddGroup("Reverse View", "ReverseView", fold: true, enable: nameof(ReverseViewEnabled));
            ui.AddValue(nameof(ReverseViewDeadZone), range: (0, 1, null));
            ui.AddValue(nameof(ReverseViewSpeedTrigger), range: (0, null, null));
            ui.EndGroup();
            ui.AddGroup("Orbit Target", "Orbit", fold: true, enable: nameof(OrbitEnabled));
            ui.AddGroup("Orbit Return", "OrbitReturn", fold: true, enable: nameof(OrbitReturnEnabled));
            ui.AddValue(nameof(OrbitReturnDelay), range: (0, null, null));
            ui.AddValue(nameof(OrbitReturnSpeed), range: (0, null, null));
            ui.EndGroup();
            ui.AddValue(nameof(OrbitSensitivity), range: (.001f, .009f, .001f));
            ui.AddCheck(nameof(OrbitInvertMouseY));
            ui.AddValue(nameof(OrbitMaxPitch), range: (Mathf.DegToRad(0), Mathf.DegToRad(90), null));
            ui.AddValue(nameof(OrbitMinPitch), range: (Mathf.DegToRad(-90), Mathf.DegToRad(0), null));
            ui.EndGroup();
            ui.EndGroup();
            ui.AddGroup("Camera Shake", "Shake", fold: true, enable: nameof(ShakeEnabled));
            ui.AddValue(nameof(ShakeRoughness), range: (0, null, null));
            ui.AddValue(nameof(ShakeScreenMultiplier), range: (0, 1, null));
            ui.AddValue(nameof(ShakeCameraMultiplier), range: (0, 1, null));
            //ui.AddNoise(nameof(ShakeNoise), fold: true, nullable: false);
            ui.EndGroup();
            ui.AddGroup("Camera Shape", "Shape", fold: true);
            ui.AddOption<ShapeType>(nameof(ShapeType));
            ui.AddValue(nameof(ShapeExpand), range: (0, 1, null));
            ui.EndGroup();
        }
    }
}
