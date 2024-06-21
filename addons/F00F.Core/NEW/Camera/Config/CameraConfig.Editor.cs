#if TOOLS
using Godot;
using Godot.Collections;

namespace F00F;

public partial class CameraConfig
{
    [ExportGroup("Camera Shape", "Shape")]
    [Export] public bool ShapeShow { get; private set => this.Set(ref field, value, ShapeChanged.Run); } = true;

    public sealed override void _ValidateProperty(Dictionary property)
    {
        if (Editor.Show(property, PropertyName.FreeCamMoveSpeed, @if: FreeCamEnabled)) return;
        if (Editor.Show(property, PropertyName.FreeCamSensitivity, @if: FreeCamEnabled)) return;
        if (Editor.Show(property, PropertyName.FreeCamInvertMouseY, @if: FreeCamEnabled)) return;
        if (Editor.Show(property, PropertyName.FreeCamMaxPitch, @if: FreeCamEnabled)) return;
        if (Editor.Show(property, PropertyName.FreeCamMinPitch, @if: FreeCamEnabled)) return;

        if (Editor.Show(property, PropertyName.ZoomMode, @if: ZoomEnabled)) return;
        if (Editor.Show(property, PropertyName.ZoomMaxSpeed, @if: ZoomEnabled)) return;
        if (Editor.Show(property, PropertyName.ZoomMaxDistance, @if: ZoomEnabled)) return;

        if (Editor.Show(property, PropertyName.LookBehindSpeed, @if: LookBehindEnabled)) return;

        if (Editor.Show(property, PropertyName.OrbitReturnEnabled, @if: OrbitEnabled)) return;
        if (Editor.Show(property, PropertyName.OrbitReturnDelay, @if: OrbitEnabled && OrbitReturnEnabled)) return;
        if (Editor.Show(property, PropertyName.OrbitReturnSpeed, @if: OrbitEnabled && OrbitReturnEnabled)) return;
        if (Editor.Show(property, PropertyName.OrbitSensitivity, @if: OrbitEnabled)) return;
        if (Editor.Show(property, PropertyName.OrbitInvertMouseY, @if: OrbitEnabled)) return;
        if (Editor.Show(property, PropertyName.OrbitMaxPitch, @if: OrbitEnabled)) return;
        if (Editor.Show(property, PropertyName.OrbitMinPitch, @if: OrbitEnabled)) return;

        if (Editor.Show(property, PropertyName.ShakeRoughness, @if: ShakeEnabled)) return;
        if (Editor.Show(property, PropertyName.ShakeScreenMultiplier, @if: ShakeEnabled)) return;
        if (Editor.Show(property, PropertyName.ShakeCameraMultiplier, @if: ShakeEnabled)) return;
        if (Editor.Show(property, PropertyName.ShakeNoise, @if: ShakeEnabled)) return;

        if (Editor.SetDisplayOnly(property, PropertyName.ShapeShow)) return;
    }
}
#endif
