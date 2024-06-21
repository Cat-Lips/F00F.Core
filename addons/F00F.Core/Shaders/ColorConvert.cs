using Godot;

namespace F00F;

[Tool]
public partial class ColorConvert : ShaderMaterial
{
    [Export(PropertyHint.ColorNoAlpha)] public Color ColorFrom { get; set => this.Set(ref field, value, OnColorFromSet); } = Colors.White;
    [Export(PropertyHint.ColorNoAlpha)] public Color ColorTo { get; set => this.Set(ref field, value, OnColorToSet); } = Colors.White;

    private void OnColorFromSet()
        => SetShaderParameter(Param.From, ColorFrom);

    private void OnColorToSet()
        => SetShaderParameter(Param.To, ColorTo);

    private static class Param
    {
        public static readonly StringName From = "color_from";
        public static readonly StringName To = "color_to";
    }
}
