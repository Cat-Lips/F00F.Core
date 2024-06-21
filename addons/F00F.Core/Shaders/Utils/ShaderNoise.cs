using System.Collections.Generic;
using Godot;

namespace F00F;

using static ShaderNoise.Enum;
using FNL = _FNL_.FastNoiseLite;

[Tool, GlobalClass]
public partial class ShaderNoise : CustomResource
{
    #region Private

    private readonly FNL fnl = new();
    private readonly FNL warp = new();

    #endregion

    #region Enums

    public static class Enum
    {
        public enum NoiseType
        {
            Value = 5,
            Perlin = 3,
            Simplex = 0,
            Cellular = 2,
            ValueCubic = 4,
            SimplexSmooth = 1,
        };

        public enum FractalType
        {
            None,
            FBm,
            Ridged,
            PingPong,
        };

        public enum DomainWarpType
        {
            None = -1,
            Simplex,
            SimplexReduced,
            BasicGrid,
        };

        public enum DomainWarpStyle
        {
            Single,
            Progressive = 4,
            Independent = 5
        };

        public enum CellularReturnType
        {
            CellValue,
            Distance,
            Distance2,
            Distance2Add,
            Distance2Sub,
            Distance2Mul,
            Distance2Div,
        };

        public enum CellularDistanceFunction
        {
            Euclidean,
            EuclideanSquared,
            Manhattan,
            Hybrid,
        };

        public enum RotationType3D
        {
            None,
            ImproveXYPlanes,
            ImproveXZPlanes,
        };
    }

    #endregion

    #region Defaults

    public static class Default
    {
        public const int Seed = 0;
        public const float Frequency = 0.01f;
        public static readonly Vector3 Offset;
        public const NoiseType NoiseType = NoiseType.Perlin;
        public const FractalType FractalType = FractalType.None;
        public const DomainWarpType DomainWarpType = DomainWarpType.None;
        public const DomainWarpStyle DomainWarpStyle = DomainWarpStyle.Single;

        public const int FractalOctaves = 3;
        public const float FractalLacunarity = 2.0f;
        public const float FractalGain = 0.5f;
        public const float FractalWeightedStrength = 0.0f;
        public const float FractalPingPongStrength = 2.0f;

        public const float DomainWarpAmplitude = 50.0f;
        public static float DomainWarpFrequency => Frequency;
        public static int DomainWarpOctaves => FractalOctaves;
        public static float DomainWarpLacunarity => FractalLacunarity;
        public static float DomainWarpGain => FractalGain;

        public const float CellularJitter = 1.0f;
        public const CellularReturnType CellularReturnType = CellularReturnType.Distance;
        public const CellularDistanceFunction CellularDistanceFunction = CellularDistanceFunction.EuclideanSquared;

        public const bool Sample3D = false;
        public const RotationType3D RotationType = RotationType3D.None;
    }

    #endregion

    #region Export

    [ExportGroup("Noise")]
    [Export] public int Seed { get; set => this.Set(ref field, value, OnSeedSet); } = Default.Seed;
    [Export] public float Frequency { get; set => this.Set(ref field, value, OnFrequencySet); } = Default.Frequency;
    [Export] public Vector3 Offset { get; set => this.Set(ref field, value, OnOffsetSet); } = Default.Offset;
    [Export] public NoiseType NoiseType { get; set => this.Set(ref field, value, notify: true, OnNoiseTypeSet); } = Default.NoiseType;
    [Export] public FractalType FractalType { get; set => this.Set(ref field, value, notify: true, OnFractalTypeSet); } = Default.FractalType;
    [Export] public DomainWarpType DomainWarpType { get; set => this.Set(ref field, value, notify: true, OnDomainWarpTypeSet); } = Default.DomainWarpType;
    [Export] public DomainWarpStyle DomainWarpStyle { get; set => this.Set(ref field, value, notify: true, OnDomainWarpStyleSet); } = Default.DomainWarpStyle;

    [ExportSubgroup("Fractal Settings", "Fractal")]
    [Export(PropertyHint.Range, "1,8")] public int FractalOctaves { get; set => this.Set(ref field, value, OnFractalOctavesSet); } = Default.FractalOctaves;
    [Export] public float FractalLacunarity { get; set => this.Set(ref field, value, OnFractalLacunaritySet); } = Default.FractalLacunarity;
    [Export] public float FractalGain { get; set => this.Set(ref field, value, OnFractalGainSet); } = Default.FractalGain;
    [Export(PropertyHint.Range, "0,1")] public float FractalWeightedStrength { get; set => this.Set(ref field, value, OnFractalWeightedStrengthSet); } = Default.FractalWeightedStrength;
    [Export] public float FractalPingPongStrength { get; set => this.Set(ref field, value, OnFractalPingPongStrengthSet); } = Default.FractalPingPongStrength;

    [ExportSubgroup("DomainWarp Settings", "DomainWarp")]
    [Export] public float DomainWarpAmplitude { get; set => this.Set(ref field, value, OnDomainWarpAmplitudeSet); } = Default.DomainWarpAmplitude;
    [Export] public float DomainWarpFrequency { get; set => this.Set(ref field, value, OnDomainWarpFrequencySet); } = Default.DomainWarpFrequency;
    [Export(PropertyHint.Range, "1,8")] public int DomainWarpOctaves { get; set => this.Set(ref field, value, OnDomainWarpOctavesSet); } = Default.DomainWarpOctaves;
    [Export] public float DomainWarpLacunarity { get; set => this.Set(ref field, value, OnDomainWarpLacunaritySet); } = Default.DomainWarpLacunarity;
    [Export] public float DomainWarpGain { get; set => this.Set(ref field, value, OnDomainWarpGainSet); } = Default.DomainWarpGain;

    [ExportSubgroup("Cellular Settings", "Cellular")]
    [Export(PropertyHint.Range, "0,1")] public float CellularJitter { get; set => this.Set(ref field, value, OnCellularJitterSet); } = Default.CellularJitter;
    [Export] public CellularReturnType CellularReturnType { get; set => this.Set(ref field, value, OnCellularReturnTypeSet); } = Default.CellularReturnType;
    [Export] public CellularDistanceFunction CellularDistanceFunction { get; set => this.Set(ref field, value, OnCellularDistanceFunctionSet); } = Default.CellularDistanceFunction;

    [ExportSubgroup("3D Settings")]
    [Export] public bool Sample3D { get; set => this.Set(ref field, value, notify: true, OnSample3DSet); } = Default.Sample3D;
    [Export] public RotationType3D RotationType { get; set => this.Set(ref field, value, OnRotationTypeSet); } = Default.RotationType;

    [ExportGroup("Shader")]
    [Export] public ShaderMaterial ShaderMaterial { get; set => this.Set(ref field, value, OnShaderMaterialSet); }
    private string ShaderCode { get; set => this.Set(ref field, value, OnShaderCodeSet); }
    private Shader Shader { get; set => this.Set(ref field, value, OnShaderSet); }

    #endregion

    public float GetNoise(in Vector2 p) => GetNoise(p.X, p.Y);
    public float GetNoise(float x, float y)
    {
        x += Offset.X; y += Offset.Y;
        if (DomainWarpType != DomainWarpType.None)
            warp.DomainWarp(ref x, ref y);
        return fnl.GetNoise(x, y);
    }

    public float GetNoise(in Vector3 p) => GetNoise(p.X, p.Y, p.Z);
    public float GetNoise(float x, float y, float z)
    {
        x += Offset.X; y += Offset.Y; z += Offset.Z;
        if (DomainWarpType != DomainWarpType.None)
            warp.DomainWarp(ref x, ref y, ref z);
        return fnl.GetNoise(x, y, z);
    }

    #region Private

    #region Init

    public ShaderNoise()
    {
        InitNoise();
        InitShader();

        void InitNoise()
        {
            SetSeed();
            SetFrequency();
            SetNoiseType();
            SetFractalType();
            SetDomainWarpType();
            SetDomainWarpStyle();

            SetFractalGain();
            SetFractalOctaves();
            SetFractalLacunarity();
            SetFractalWeightedStrength();
            SetFractalPingPongStrength();

            SetDomainWarpAmp();
            SetDomainWarpFrequency();
            SetDomainWarpOctaves();
            SetDomainWarpLacunarity();
            SetDomainWarpGain();

            SetCellularJitter();
            SetCellularReturnType();
            SetCellularDistanceFunction();

            SetRotationType();
        }

        void InitShader()
        {
            DisableChangedEvent();
            ShaderMaterial = New.ShaderMaterial();
            EnableChangedEvent();
        }
    }

    #endregion

    #region Noise

    private void SetSeed() { fnl.SetSeed(Seed); warp.SetSeed(Seed); }
    private void SetFrequency() => fnl.SetFrequency(Frequency);
    private void SetNoiseType() => fnl.SetNoiseType((FNL.NoiseType)NoiseType);
    private void SetFractalType() => fnl.SetFractalType((FNL.FractalType)FractalType);
    private void SetDomainWarpType() => warp.SetDomainWarpType((FNL.DomainWarpType)DomainWarpType);
    private void SetDomainWarpStyle() => warp.SetFractalType((FNL.FractalType)DomainWarpStyle);

    private void SetFractalGain() => fnl.SetFractalGain(FractalGain);
    private void SetFractalOctaves() => fnl.SetFractalOctaves(FractalOctaves);
    private void SetFractalLacunarity() => fnl.SetFractalLacunarity(FractalLacunarity);
    private void SetFractalWeightedStrength() => fnl.SetFractalWeightedStrength(FractalWeightedStrength);
    private void SetFractalPingPongStrength() => fnl.SetFractalPingPongStrength(FractalPingPongStrength);

    private void SetDomainWarpAmp() => warp.SetDomainWarpAmp(DomainWarpAmplitude);
    private void SetDomainWarpFrequency() => warp.SetFrequency(DomainWarpFrequency);
    private void SetDomainWarpOctaves() => warp.SetFractalOctaves(DomainWarpOctaves);
    private void SetDomainWarpLacunarity() => warp.SetFractalLacunarity(DomainWarpLacunarity);
    private void SetDomainWarpGain() => warp.SetFractalGain(DomainWarpGain);

    private void SetCellularJitter() => fnl.SetCellularJitter(CellularJitter);
    private void SetCellularReturnType() => fnl.SetCellularReturnType((FNL.CellularReturnType)CellularReturnType);
    private void SetCellularDistanceFunction() => fnl.SetCellularDistanceFunction((FNL.CellularDistanceFunction)CellularDistanceFunction);

    private void SetRotationType() { fnl.SetRotationType3D((FNL.RotationType3D)RotationType); warp.SetRotationType3D((FNL.RotationType3D)RotationType); }

    #endregion

    private void OnSeedSet() { SetSeed(); ShaderMaterial.SetShaderParameter(Param.Seed, Seed); }
    private void OnFrequencySet() { SetFrequency(); ShaderMaterial.SetShaderParameter(Param.Frequency, Frequency); }
    private void OnNoiseTypeSet() { CompileShader(); SetNoiseType(); ShaderMaterial.SetShaderParameter(Param.NoiseType, (int)NoiseType); }
    private void OnFractalTypeSet() { CompileShader(); SetFractalType(); ShaderMaterial.SetShaderParameter(Param.FractalType, (int)FractalType); }
    private void OnDomainWarpTypeSet() { CompileShader(); SetDomainWarpType(); ShaderMaterial.SetShaderParameter(Param.DomainWarpType, (int)DomainWarpType); }
    private void OnDomainWarpStyleSet() { CompileShader(); SetDomainWarpStyle(); ShaderMaterial.SetShaderParameter(Param.DomainWarpStyle, (int)DomainWarpStyle); }

    private void OnFractalGainSet() { SetFractalGain(); ShaderMaterial.SetShaderParameter(Param.FractalGain, FractalGain); }
    private void OnFractalOctavesSet() { SetFractalOctaves(); ShaderMaterial.SetShaderParameter(Param.FractalOctaves, FractalOctaves); }
    private void OnFractalLacunaritySet() { SetFractalLacunarity(); ShaderMaterial.SetShaderParameter(Param.FractalLacunarity, FractalLacunarity); }
    private void OnFractalWeightedStrengthSet() { SetFractalWeightedStrength(); ShaderMaterial.SetShaderParameter(Param.FractalWeightedStrength, FractalWeightedStrength); }
    private void OnFractalPingPongStrengthSet() { SetFractalPingPongStrength(); ShaderMaterial.SetShaderParameter(Param.FractalPingPongStrength, FractalPingPongStrength); }

    private void OnDomainWarpAmplitudeSet() { SetDomainWarpAmp(); ShaderMaterial.SetShaderParameter(Param.DomainWarpAmplitude, DomainWarpAmplitude); }
    private void OnDomainWarpFrequencySet() { SetDomainWarpFrequency(); ShaderMaterial.SetShaderParameter(Param.DomainWarpFrequency, DomainWarpFrequency); }
    private void OnDomainWarpOctavesSet() { SetDomainWarpOctaves(); ShaderMaterial.SetShaderParameter(Param.DomainWarpOctaves, DomainWarpOctaves); }
    private void OnDomainWarpLacunaritySet() { SetDomainWarpLacunarity(); ShaderMaterial.SetShaderParameter(Param.DomainWarpLacunarity, DomainWarpLacunarity); }
    private void OnDomainWarpGainSet() { SetDomainWarpGain(); ShaderMaterial.SetShaderParameter(Param.DomainWarpGain, DomainWarpGain); }

    private void OnCellularJitterSet() { fnl.SetCellularJitter(CellularJitter); ShaderMaterial.SetShaderParameter(Param.CellularJitter, CellularJitter); }
    private void OnCellularReturnTypeSet() { fnl.SetCellularReturnType((FNL.CellularReturnType)CellularReturnType); ShaderMaterial.SetShaderParameter(Param.CellularReturnType, (int)CellularReturnType); }
    private void OnCellularDistanceFunctionSet() { fnl.SetCellularDistanceFunction((FNL.CellularDistanceFunction)CellularDistanceFunction); ShaderMaterial.SetShaderParameter(Param.CellularDistanceFunction, (int)CellularDistanceFunction); }

    private void OnSample3DSet() => CompileShader();
    private void OnRotationTypeSet() { SetRotationType(); ShaderMaterial.SetShaderParameter(Param.RotationType, (int)RotationType); }
    private void OnOffsetSet() { ShaderMaterial.SetShaderParameter(Param.Offset, Offset.XY()); ShaderMaterial.SetShaderParameter(Param.Offset3D, Offset); }

    #region Shader

    private void OnShaderMaterialSet(ShaderMaterial old, ShaderMaterial _)
    {
        SetShaderParameters();
        OnShaderMaterialChanged();
        old?.Changed -= OnShaderMaterialChanged;
        ShaderMaterial.Changed += OnShaderMaterialChanged;

        void SetShaderParameters()
        {
            ShaderMaterial.SetShaderParameter(Param.Seed, Seed);
            ShaderMaterial.SetShaderParameter(Param.Frequency, Frequency);
            ShaderMaterial.SetShaderParameter(Param.NoiseType, (int)NoiseType);
            ShaderMaterial.SetShaderParameter(Param.FractalType, (int)FractalType);
            ShaderMaterial.SetShaderParameter(Param.DomainWarpType, (int)DomainWarpType);
            ShaderMaterial.SetShaderParameter(Param.DomainWarpStyle, (int)DomainWarpStyle);

            ShaderMaterial.SetShaderParameter(Param.FractalGain, FractalGain);
            ShaderMaterial.SetShaderParameter(Param.FractalOctaves, FractalOctaves);
            ShaderMaterial.SetShaderParameter(Param.FractalLacunarity, FractalLacunarity);
            ShaderMaterial.SetShaderParameter(Param.FractalWeightedStrength, FractalWeightedStrength);
            ShaderMaterial.SetShaderParameter(Param.FractalPingPongStrength, FractalPingPongStrength);

            ShaderMaterial.SetShaderParameter(Param.DomainWarpAmplitude, DomainWarpAmplitude);
            ShaderMaterial.SetShaderParameter(Param.DomainWarpFrequency, DomainWarpFrequency);
            ShaderMaterial.SetShaderParameter(Param.DomainWarpOctaves, DomainWarpOctaves);
            ShaderMaterial.SetShaderParameter(Param.DomainWarpLacunarity, DomainWarpLacunarity);
            ShaderMaterial.SetShaderParameter(Param.DomainWarpGain, DomainWarpGain);

            ShaderMaterial.SetShaderParameter(Param.CellularJitter, CellularJitter);
            ShaderMaterial.SetShaderParameter(Param.CellularReturnType, (int)CellularReturnType);
            ShaderMaterial.SetShaderParameter(Param.CellularDistanceFunction, (int)CellularDistanceFunction);

            ShaderMaterial.SetShaderParameter(Param.Offset, Offset.XY());
            ShaderMaterial.SetShaderParameter(Param.Offset3D, Offset);
            ShaderMaterial.SetShaderParameter(Param.RotationType, (int)RotationType);
        }

        void OnShaderMaterialChanged()
            => Shader = ShaderMaterial.Shader;
    }

    private void OnShaderSet(Shader old, Shader _)
    {
        OnShaderChanged();
        old?.Changed -= OnShaderChanged;
        Shader?.Changed += OnShaderChanged;

        void OnShaderChanged()
        {
            if (wip) return;
            ShaderCode = Shader?.Code;
        }
    }

    private void OnShaderCodeSet()
        => CompileShader();

    private bool wip;
    private void CompileShader()
    {
        try { wip = true; ShaderMaterial.Shader = new() { Code = string.Join("\n", Parts()) }; }
        finally { wip = false; }

        IEnumerable<string> Parts()
        {
            if (Sample3D) yield return "#define FNL_USE_3D";
            yield return $"#define FNL_USE_{GetNoiseType()}";
            if (FractalType is not FractalType.None) yield return $"#define FNL_USE_{GetFractalType()}";
            if (DomainWarpType is not DomainWarpType.None) yield return $"#define FNL_USE_DOMAINWARP_{GetWarpType()}";
            if (DomainWarpType is not DomainWarpType.None && DomainWarpStyle is not DomainWarpStyle.Single) yield return $"#define FNL_USE_DOMAINWARP_{GetWarpStyle()}";
            yield return string.Empty;
            yield return ShaderCode;

            string GetNoiseType() => NoiseType switch
            {
                NoiseType.Simplex => "SIMPLEX2",
                NoiseType.SimplexSmooth => "SIMPLEX2S",
                _ => $"{NoiseType}".ToUpperInvariant(),
            };

            string GetFractalType() => $"{FractalType}".ToUpperInvariant();

            string GetWarpType() => DomainWarpType switch
            {
                DomainWarpType.Simplex => "SIMPLEX2",
                DomainWarpType.SimplexReduced => "SIMPLEX2S",
                _ => $"{DomainWarpType}".ToUpperInvariant(),
            };

            string GetWarpStyle() => $"{DomainWarpStyle}".ToUpperInvariant();
        }
    }

    #endregion

    #endregion

    #region Params

    private static class Param
    {
        public static readonly StringName Seed = "Seed";
        public static readonly StringName Frequency = "Frequency";
        public static readonly StringName NoiseType = "NoiseType";

        public static readonly StringName FractalType = "FractalType";
        public static readonly StringName FractalOctaves = "FractalOctaves";
        public static readonly StringName FractalLacunarity = "FractalLacunarity";
        public static readonly StringName FractalGain = "FractalGain";
        public static readonly StringName FractalWeightedStrength = "FractalWeightedStrength";
        public static readonly StringName FractalPingPongStrength = "FractalPingPongStrength";

        public static readonly StringName DomainWarpType = "DomainWarpType";
        public static readonly StringName DomainWarpStyle = "DomainWarpStyle";
        public static readonly StringName DomainWarpAmplitude = "DomainWarpAmplitude";
        public static readonly StringName DomainWarpFrequency = "DomainWarpFrequency";
        public static readonly StringName DomainWarpOctaves = "DomainWarpOctaves";
        public static readonly StringName DomainWarpLacunarity = "DomainWarpLacunarity";
        public static readonly StringName DomainWarpGain = "DomainWarpGain";

        public static readonly StringName CellularJitter = "CellularJitter";
        public static readonly StringName CellularReturnType = "CellularReturnType";
        public static readonly StringName CellularDistanceFunction = "CellularDistanceFunction";

        public static readonly StringName Offset = "Offset";
        public static readonly StringName Offset3D = "Offset3D";
        public static readonly StringName RotationType = "RotationType3D";
    }

    #endregion
}
