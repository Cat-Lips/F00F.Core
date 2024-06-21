using Godot;

namespace F00F;

public static class ColorExtensions
{
    public static Color With(this in Color source, float? r = null, float? g = null, float? b = null, float? a = null)
        => new(r ?? source.R, g ?? source.G, b ?? source.B, a ?? source.A);
}
