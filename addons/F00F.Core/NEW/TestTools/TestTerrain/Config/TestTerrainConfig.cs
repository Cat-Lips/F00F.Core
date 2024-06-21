using Godot;

namespace F00F;

using static TestTerrainConfig.Enum;

[Tool]
public partial class TestTerrainConfig : CustomResource
{
    #region Enums

    public static class Enum
    {
        public enum MeshType { Polygon, Shader }
        public enum ShapeType { Polygon, HeightMap }
    }

    #endregion

    #region Defaults

    public static class Default
    {
        public static Texture2D HeightMap => New.NoiseTexture(/*smooth: true*/);
        public static Gradient ColorMap => New.Gradient(colors);
        public static Curve Gradient => New.Curve(slopes);
        public static ShapeType ShapeType => ShapeType.HeightMap;
        public static MeshType MeshType => MeshType.Polygon;
        public static int Amplitude => 25;

        private static readonly (float, Color)[] colors =
        [
            (0.0f, new(0.0392157f, 0.164706f, 0.262745f)),  // Deep water
            (0.1f, new(0.117647f, 0.478431f, 0.580392f)),   // Shallow water
            (0.2f, new(0.854902f, 0.776471f, 0.588235f)),   // Sand
            (0.3f, new(0.431373f, 0.560784f, 0.227451f)),   // Grass
            (0.7f, new(0.215686f, 0.305882f, 0.149020f)),   // Forest
            (0.8f, new(0.478431f, 0.435294f, 0.364706f)),   // Low Rock
            (0.9f, new(0.352941f, 0.368627f, 0.388235f)),   // High Rock
            (1.0f, new(0.949020f, 0.960784f, 0.968627f)),   // Snow
        ];

        private static readonly (float, float)[] slopes =
        [
            (0.0f, 0.9f),   // Deep water
            (0.1f, 0.6f),   // Shallow water
            (0.2f, 0.2f),   // Sand
            (0.3f, 0.1f),   // Grass
            (0.7f, 0.3f),   // Forest
            (0.8f, 0.5f),   // Low Rock
            (0.9f, 0.8f),   // High Rock
            (1.0f, 0.9f),   // Snow
        ];
    }

    #endregion

    public readonly AutoAction HeightMapChanged = new();
    public readonly AutoAction ColorMapChanged = new();
    public readonly AutoAction GradientChanged = new();
    public readonly AutoAction ShapeTypeChanged = new();
    public readonly AutoAction MeshTypeChanged = new();
    public readonly AutoAction AmplitudeChanged = new();

    #region Exports

    [Export] public Texture2D HeightMap { get; set => this.OnChanged(ref field, value ?? Default.HeightMap, HeightMapChanged.Run); }
    [Export] public Gradient ColorMap { get; set => this.OnChanged(ref field, value ?? Default.ColorMap, ColorMapChanged.Run); }
    [Export] public Curve Gradient { get; set => this.OnChanged(ref field, value ?? Default.Gradient, GradientChanged.Run); }
    [Export] public ShapeType ShapeType { get; set => this.OnChanged(ref field, value, ShapeTypeChanged.Run); }
    [Export] public MeshType MeshType { get; set => this.OnChanged(ref field, value, MeshTypeChanged.Run); }
    [Export] public int Amplitude { get; set => this.OnChanged(ref field, value.ClampMin(1), AmplitudeChanged.Run); }

    #endregion

    #region Private

    public TestTerrainConfig()
    {
        HeightMap = Default.HeightMap;
        ColorMap = Default.ColorMap;
        Gradient = Default.Gradient;
        ShapeType = Default.ShapeType;
        MeshType = Default.MeshType;
        Amplitude = Default.Amplitude;
    }

    #endregion
}
