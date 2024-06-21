using System.Runtime.CompilerServices;
using Godot;

namespace F00F;

[Tool, GlobalClass]
public partial class CityCUR : WorldConfig
{
    #region Defaults

    public static class Default
    {
        public const float Scale = 1.0f;

        public const float RoadThreshold = 0.05f;
        public const float BermThreshold = 0.15f;

        public const float RoadHeight = 0.0f;
        public const float BermHeight = 0.5f;
        public const float BaseHeight = 15.0f;

        public static readonly Color RoadColor = new(1.0f, 0.0f, 0.0f);   // R
        public static readonly Color BermColor = new(0.0f, 1.0f, 0.0f);   // G
        public static readonly Color BaseColor = new(0.0f, 0.0f, 1.0f);   // B

        //public static readonly Color RoadColor = new(0.77f, 0.64f, 0.35f);	// #C4A35A	Bright warm sandstone — worn road surface
        //public static readonly Color BermColor = new(0.55f, 0.48f, 0.32f);	// #8C7A52	Muted stone — raised walkways/curbs
        //public static readonly Color BaseColor = new(0.42f, 0.26f, 0.15f);	// #6B4226	Dark clay — mudbrick structures

        public const float NormalStrength = 1.0f;
        public const float MinDetailDist = 0.5f;
        public const float MaxDetailDist = 25.0f;
    }

    #endregion

    #region Export

    [Export] public int Seed { get; set => this.Set(ref field, value, () => SetShaderParam()); }
    [Export] public Vector2 Offset { get; set => this.Set(ref field, value, () => SetShaderParam()); }
    [Export(PropertyHint.Range, "0.1,10.0")] public float Scale { get; set => this.Set(ref field, value, () => SetShaderParam()); }

    [ExportGroup("Thresholds")]
    [Export(PropertyHint.Range, "0.0, 5.0")] public float RoadThreshold { get; set => this.Set(ref field, value, () => SetShaderParam()); }
    [Export(PropertyHint.Range, "0.0, 5.0")] public float BermThreshold { get; set => this.Set(ref field, value, () => SetShaderParam()); }

    [ExportGroup("Heights")]
    [Export(PropertyHint.Range, "0.0, 5.0")] public float RoadHeight { get; set => this.Set(ref field, value, () => SetShaderParam()); }
    [Export(PropertyHint.Range, "0.0, 5.0")] public float BermHeight { get; set => this.Set(ref field, value, () => SetShaderParam()); }
    [Export(PropertyHint.Range, "0.0, 50.0")] public float BaseHeight { get; set => this.Set(ref field, value, () => SetShaderParam()); }

    [ExportGroup("Colors")]
    [Export(PropertyHint.ColorNoAlpha)] public Color RoadColor { get; set => this.Set(ref field, value, () => SetShaderParam()); }
    [Export(PropertyHint.ColorNoAlpha)] public Color BermColor { get; set => this.Set(ref field, value, () => SetShaderParam()); }
    [Export(PropertyHint.ColorNoAlpha)] public Color BaseColor { get; set => this.Set(ref field, value, () => SetShaderParam()); }

    [ExportGroup("Textures")]
    [Export] public TextureLibrary Textures { get; set => this.Set(ref field, value, SetTextureParams); }

    [ExportGroup("Tuning")]
    [Export(PropertyHint.Range, "0.0, 2.0")] public float NormalStrength { get; set => this.Set(ref field, value, () => SetShaderParam()); }
    [Export(PropertyHint.Range, "0.0, 5.0")] public float MinDetailDist { get; set => this.Set(ref field, value, () => SetShaderParam()); }
    [Export(PropertyHint.Range, "5.0, 50.0")] public float MaxDetailDist { get; set => this.Set(ref field, value, () => SetShaderParam()); }

    #endregion

    #region Private

    private void SetShaderParam([CallerMemberName] string caller = null)
        => ShaderMaterial.SetShaderParameter(caller.ToSnakeCase(), Get(caller));

    private void SetTextureParams()
        => Textures.Set(ShaderMaterial);

    #endregion
}
