using System;
using Godot;

namespace F00F;

using static TestTerrain.Enum;
using Camera = Godot.Camera3D;

[Tool]
public partial class TestTerrain : Node3D
{
    public record struct State(Vector2 Position, float Height, Color Color, float Gradient, float Altitude);

    #region Private

    private Camera Camera => field ??= GetViewport().GetCamera3D();

    private WorldBounds Bounds => field ??= GetNode<WorldBounds>("Bounds");
    private MeshInstance3D Mesh => field ??= GetNode<MeshInstance3D>("Mesh");
    private CollisionShape3D Shape => field ??= GetNode<CollisionShape3D>("Shape");
    private Marker3D SpawnPoint => field ??= GetNode<Marker3D>("SpawnPoint");

    private PlaneMesh Plane => (PlaneMesh)Mesh.Mesh;
    private ShaderMaterial Shader => (ShaderMaterial)Plane.Material;

    #endregion

    public event Action<Vector3> SpawnReady;

    public Vector2I Size { get; private set; }
    public float[,] Data { get; private set; }

    #region Export

    public static class Enum
    {
        public enum MeshType { Polygon, Shader }
        public enum ShapeType { Polygon, HeightMap }
    }

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

    [Export] public Texture2D HeightMap { get; set => this.Set(ref field, value ?? Default.HeightMap, OnHeightMapSet); }
    [Export] public Gradient ColorMap { get; set => this.Set(ref field, value ?? Default.ColorMap, OnColorMapSet); }
    [Export] public Curve Gradient { get; set => this.Set(ref field, value ?? Default.Gradient, OnGradientSet); }
    [Export] public ShapeType ShapeType { get; set => this.Set(ref field, value, OnShapeTypeSet); } = Default.ShapeType;
    [Export] public MeshType MeshType { get; set => this.Set(ref field, value, OnMeshTypeSet); } = Default.MeshType;
    [Export] public int Amplitude { get; set => this.Set(ref field, value.ClampMin(1), OnAmplitudeSet); } = Default.Amplitude;

    #endregion

    public float GetHeight(in Vector3 pos) => GetHeight(pos.XZ());
    public float GetHeight(in Vector2 xz) => GetHeight(xz.X, xz.Y);
    public float GetHeight(float x, float z)
    {
        var h = Data.Value(x, z);
        var g = Gradient.Sample(h);
        return h * g * Amplitude;
    }

    public State GetState(in Vector3 pos)
    {
        var p = pos.XZ();
        var h = Data.Value(p);
        var c = ColorMap.Sample(h);
        var g = Gradient.Sample(h);
        var height = h * g * Amplitude;
        return new(p, height, c, g, pos.Y - height);
    }

    #region Godot

    private bool IsReady { get; set; }
    private bool HasSpawned { get; set; }
    public sealed override void _Ready()
    {
        Editor.Disable(this);

        HeightMap ??= Default.HeightMap;
        ColorMap ??= Default.ColorMap;
        Gradient ??= Default.Gradient;

        IsReady = true;

        ResetData();
    }

    public override void _Process(double _)
        => this.Clamp(Camera, Camera.Near);

    #endregion

    #region Private

    private void OnHeightMapSet()
    {
        if (IsReady)
            ResetData();
    }

    private void OnColorMapSet()
    {
        if (IsReady)
        {
            switch (MeshType)
            {
                case MeshType.Polygon:
                    ResetMesh();
                    break;
                case MeshType.Shader:
                    SetColorMap();
                    break;
            }
        }
    }

    private void OnGradientSet()
    {
        if (IsReady)
        {
            switch (MeshType)
            {
                case MeshType.Polygon:
                    ResetMesh();
                    ResetShape();
                    break;
                case MeshType.Shader:
                    ResetShape();
                    SetGradient();
                    break;
            }
        }
    }

    private void OnShapeTypeSet()
    {
        if (IsReady)
            ResetShape();
    }

    private void OnMeshTypeSet()
    {
        if (IsReady)
        {
            switch (MeshType)
            {
                case MeshType.Polygon:
                    ResetMesh();
                    break;
                case MeshType.Shader:
                    ResetMesh();
                    SetColorMap();
                    SetGradient();
                    SetAmplitude();
                    SetHeightMap();
                    break;
            }
        }
    }

    private void OnAmplitudeSet()
    {
        if (IsReady)
        {
            switch (MeshType)
            {
                case MeshType.Polygon:
                    ResetMesh();
                    ResetShape();
                    break;
                case MeshType.Shader:
                    ResetShape();
                    SetAmplitude();
                    break;
            }
        }
    }

    private void ResetData()
    {
        if (ResetData())
            OnDataReady();

        bool ResetData()
        {
            var img = HeightMap.GetImage();
            if (img is null) return false;

            img.Data(out var data, out var size);
            Data = data; Size = size;
            Bounds.Size = size;
            return true;
        }

        void OnDataReady()
        {
            switch (MeshType)
            {
                case MeshType.Polygon:
                    ResetMesh();
                    ResetShape();
                    SetSpawnReady();
                    break;
                case MeshType.Shader:
                    ResetMesh();
                    ResetShape();
                    SetColorMap();
                    SetGradient();
                    SetAmplitude();
                    SetHeightMap();
                    SetSpawnReady();
                    break;
            }
        }
    }

    private void ResetMesh()
    {
        Mesh.Mesh = MeshType switch
        {
            MeshType.Polygon => New.ArrayMesh(Size, Amplitude, RawHeight, RawColor, RawGradient),
            MeshType.Shader => New.PlaneMesh(Size, New.ShaderMaterial<TestTerrain>()),
            _ => throw new NotImplementedException(),
        };

        float RawHeight(float x, float z)
            => Data.Value(x, z);

        Color RawColor(float h)
            => ColorMap.Sample(h);

        float RawGradient(float h)
            => Gradient.Sample(h);
    }

    private void ResetShape()
    {
        Shape.Shape = ShapeType switch
        {
            ShapeType.Polygon => New.PolygonShape(Size, Position.XZ(), GetHeight),
            ShapeType.HeightMap => New.HeightMapShape(Size, Position.XZ(), GetHeight),
            _ => throw new NotImplementedException(),
        };
    }

    private void SetColorMap()
        => Shader.SetShaderParameter(ShaderParams.ColorMap, ColorMap.Texture());

    private void SetGradient()
        => Shader.SetShaderParameter(ShaderParams.Gradient, Gradient.Texture());

    private void SetAmplitude()
        => Shader.SetShaderParameter(ShaderParams.Amplitude, Amplitude);

    private void SetHeightMap()
        => Shader.SetShaderParameter(ShaderParams.HeightMap, HeightMap);

    private void SetSpawnReady()
    {
        if (!HasSpawned)
        {
            HasSpawned = true;
            SpawnReady?.Invoke(SpawnPoint.GlobalPosition.With(y: Amplitude));
        }
    }

    private static class ShaderParams
    {
        public static readonly StringName Gradient = "gradient";
        public static readonly StringName ColorMap = "color_map";
        public static readonly StringName Amplitude = "amplitude";
        public static readonly StringName HeightMap = "height_map";
    }

    #endregion
}
