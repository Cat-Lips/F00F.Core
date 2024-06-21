using Godot;

namespace F00F;

public interface INoiseShader
{

}

[Tool, GlobalClass]
public partial class CityShader : WorldConfig
{
    #region Defaults

    public static class Default
    {
        public static ShaderNoise Noise => new()
        {
            NoiseType = ShaderNoise.Enum.NoiseType.Cellular,
            CellularReturnType = ShaderNoise.Enum.CellularReturnType.Distance2Sub,
        };
    }

    #endregion

    #region Export

    [ExportCategory("City")]

    [ExportGroup("Layout")]
    [Export] public ShaderNoise Noise { get; set => this.Set(ref field, value ?? Default.Noise, OnNoiseSet); }

    #endregion

    #region Private

    public CityShader()
    {
        ShaderMaterial.Shader = Utils.LoadShader<CityShader>();

        DisableChangedEvent();
        Noise = Default.Noise;
        EnableChangedEvent();
    }

    private void OnNoiseSet()
        => Noise.ShaderMaterial = ShaderMaterial;

    #endregion
}
