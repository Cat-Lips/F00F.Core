using System;
using Godot;

namespace F00F;

using static TestTerrainConfig.Enum;
using Camera = Godot.Camera3D;

[Tool]
public partial class TestTerrain : Node3D
{
    public record struct State(Vector2 Position, float Height, Color Color, float Gradient, float Altitude);

    #region Private

    private Camera Camera => field ??= GetViewport().GetCamera3D();

    private WorldBounds Bounds => field ??= (WorldBounds)GetNode("Bounds");
    private MeshInstance3D Mesh => field ??= (MeshInstance3D)GetNode("Mesh");
    private CollisionShape3D Shape => field ??= (CollisionShape3D)GetNode("Shape");

    private PlaneMesh Plane => (PlaneMesh)Mesh.Mesh;
    private ShaderMaterial Shader => (ShaderMaterial)Plane.Material;

    #endregion

    public event Action<Vector3> SpawnReady;
    public Vector3? SpawnPoint { get; private set; }

    public Vector2I Size { get; private set; }
    public float[,] Data { get; private set; }

    #region Export

    [Export] public TestTerrainConfig Config { get; set => this.Set(ref field, value ?? new(), OnConfigSet); }

    #endregion

    public float GetHeight(in Vector3 pos) => GetHeight(pos.XZ());
    public float GetHeight(in Vector2 xz) => GetHeight(xz.X, xz.Y);
    public float GetHeight(float x, float z)
    {
        var h = Data.Value(x, z);
        var g = Config.Gradient.Sample(h);
        return h * g * Config.Amplitude;
    }

    public State GetState(in Vector3 pos)
    {
        var p = pos.XZ();
        var h = Data.Value(p);
        var c = Config.ColorMap.Sample(h);
        var g = Config.Gradient.Sample(h);
        var height = h * g * Config.Amplitude;
        return new(p, height, c, g, pos.Y - height);
    }

    internal float GetAltitude(Vector3 pos)
        => pos.Y - GetHeight(pos.XZ());

    #region Godot

    public sealed override void _Ready()
    {
        Editor.Disable(this);
        Config ??= new();
        ResetData();
    }

    public sealed override void _Process(double _)
        => this.Clamp(Camera, Camera.Near * 2);

    #endregion

    #region Private

    private void OnConfigSet()
    {
        Config.HeightMapChanged.Action += OnHeightMapChanged;
        Config.ColorMapChanged.Action += OnColorMapChanged;
        Config.GradientChanged.Action += OnGradientChanged;
        Config.ShapeTypeChanged.Action += OnShapeTypeChanged;
        Config.MeshTypeChanged.Action += OnMeshTypeChanged;
        Config.AmplitudeChanged.Action += OnAmplitudeChanged;

        void OnHeightMapChanged()
        {
            if (IsInstanceValid(this))
                this.OnHeightMapChanged();
        }
    }

    private void OnHeightMapChanged()
        => ResetData();

    private void OnColorMapChanged()
    {
        if (Data is null) return;
        switch (Config.MeshType)
        {
            case MeshType.Polygon:
                ResetMesh();
                break;
            case MeshType.Shader:
                SetColorMap();
                break;
        }
    }

    private void OnGradientChanged()
    {
        if (Data is null) return;
        switch (Config.MeshType)
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

    private void OnShapeTypeChanged()
    {
        if (Data is null) return;
        ResetShape();
    }

    private void OnMeshTypeChanged()
    {
        if (Data is null) return;
        switch (Config.MeshType)
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

    private void OnAmplitudeChanged()
    {
        if (Data is null) return;
        switch (Config.MeshType)
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

    private void ResetData()
    {
        if (ResetData())
            OnDataReady();

        bool ResetData()
        {
            var img = Config.HeightMap.GetImage();
            if (img is null) return false;

            img.Data(out var data, out var size);
            Data = data; Size = size;
            Bounds.Size = size;
            return true;
        }

        void OnDataReady()
        {
            switch (Config.MeshType)
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
        Mesh.Mesh = Config.MeshType switch
        {
            MeshType.Polygon => New.TerrainMesh(Size, Config.Amplitude, RawHeight, RawColor, RawGradient),
            MeshType.Shader => New.PlaneMesh(Size, New.ShaderMaterial<TestTerrain>()),
            _ => throw new NotImplementedException(),
        };

        float RawHeight(float x, float z)
            => Data.Value(x, z);

        Color RawColor(float h)
            => Config.ColorMap.Sample(h);

        float RawGradient(float h)
            => Config.Gradient.Sample(h);
    }

    private void ResetShape()
    {
        Shape.Shape = Config.ShapeType switch
        {
            ShapeType.Polygon => New.PolygonShape(Size, Position.XZ(), GetHeight),
            ShapeType.HeightMap => New.HeightMapShape(Size, Position.XZ(), GetHeight),
            _ => throw new NotImplementedException(),
        };
    }

    private void SetColorMap()
        => Shader.SetShaderParameter(ShaderParams.ColorMap, Config.ColorMap.Texture());

    private void SetGradient()
        => Shader.SetShaderParameter(ShaderParams.Gradient, Config.Gradient.Texture());

    private void SetAmplitude()
        => Shader.SetShaderParameter(ShaderParams.Amplitude, Config.Amplitude);

    private void SetHeightMap()
        => Shader.SetShaderParameter(ShaderParams.HeightMap, Config.HeightMap);

    private void SetSpawnReady()
    {
        SpawnPoint = new(0, Config.Amplitude, 0);
        SpawnReady?.Invoke(SpawnPoint.Value);
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
