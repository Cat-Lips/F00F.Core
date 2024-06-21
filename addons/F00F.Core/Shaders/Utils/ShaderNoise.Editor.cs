#if TOOLS
using Godot.Collections;

namespace F00F;

using static ShaderNoise.Enum;

public partial class ShaderNoise
{
    public override void _ValidateProperty(Dictionary property)
    {
        if (Editor.Hide(property, PropertyName.FractalGain, @if: FractalType is FractalType.None)) return;
        if (Editor.Hide(property, PropertyName.FractalOctaves, @if: FractalType is FractalType.None)) return;
        if (Editor.Hide(property, PropertyName.FractalLacunarity, @if: FractalType is FractalType.None)) return;
        if (Editor.Hide(property, PropertyName.FractalWeightedStrength, @if: FractalType is FractalType.None)) return;
        if (Editor.Show(property, PropertyName.FractalPingPongStrength, @if: FractalType is FractalType.PingPong)) return;

        if (Editor.Hide(property, PropertyName.DomainWarpStyle, @if: DomainWarpType is DomainWarpType.None)) return;
        if (Editor.Hide(property, PropertyName.DomainWarpAmplitude, @if: DomainWarpType is DomainWarpType.None)) return;
        if (Editor.Hide(property, PropertyName.DomainWarpFrequency, @if: DomainWarpType is DomainWarpType.None)) return;
        if (Editor.Hide(property, PropertyName.DomainWarpOctaves, @if: DomainWarpType is DomainWarpType.None)) return;
        if (Editor.Hide(property, PropertyName.DomainWarpLacunarity, @if: DomainWarpType is DomainWarpType.None || DomainWarpStyle is DomainWarpStyle.Single)) return;
        if (Editor.Hide(property, PropertyName.DomainWarpGain, @if: DomainWarpType is DomainWarpType.None)) return;

        if (Editor.Show(property, PropertyName.CellularJitter, @if: NoiseType is NoiseType.Cellular)) return;
        if (Editor.Show(property, PropertyName.CellularReturnType, @if: NoiseType is NoiseType.Cellular)) return;
        if (Editor.Show(property, PropertyName.CellularDistanceFunction, @if: NoiseType is NoiseType.Cellular)) return;

        if (Editor.Show(property, PropertyName.RotationType, @if: Sample3D)) return;
    }
}
#endif
