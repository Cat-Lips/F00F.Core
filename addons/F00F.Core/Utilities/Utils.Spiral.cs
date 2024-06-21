using System.Collections.Generic;
using Godot;

namespace F00F;

public static partial class Utils
{
    public static IEnumerable<(Vector2I Cell, int Ring)> Spiral(float x0, float y0, float radius) => Spiral(x0.RoundInt(), y0.RoundInt(), radius.RoundInt());
    public static IEnumerable<(Vector2I Cell, int Ring)> Spiral(float x0, float y0, int radius) => Spiral(x0.RoundInt(), y0.RoundInt(), radius);

    public static IEnumerable<(Vector2I Cell, int Ring)> Spiral(in Vector2 origin, float radius) => Spiral(origin.RoundInt(), radius.RoundInt());
    public static IEnumerable<(Vector2I Cell, int Ring)> Spiral(in Vector2 origin, int radius) => Spiral(origin.RoundInt(), radius);

    public static IEnumerable<(Vector2I Cell, int Ring)> Spiral(in Vector2I origin, float radius) => Spiral(origin.X, origin.Y, radius.RoundInt());
    public static IEnumerable<(Vector2I Cell, int Ring)> Spiral(in Vector2I origin, int radius) => Spiral(origin.X, origin.Y, radius);

    public static IEnumerable<(Vector2I Cell, int Ring)> Spiral(float radius) => Spiral(0, 0, radius.RoundInt());
    public static IEnumerable<(Vector2I Cell, int Ring)> Spiral(int radius) => Spiral(0, 0, radius);

    private static readonly Vector2I[] Sides = [new(-1, 0), new(0, -1), new(1, 0), new(0, 1)];
    public static IEnumerable<(Vector2I Cell, int Ring)> Spiral(int x0, int y0, int radius)
    {
        if (radius < 0) yield break;

        yield return (new(x0, y0), 0);

        for (var ring = 1; ring <= radius; ++ring)
        {
            var cell = new Vector2I(x0 + ring, y0 + ring);
            foreach (var side in Sides)
            {
                var count = 2 * ring;
                for (var cellIdx = 0; cellIdx < count; ++cellIdx)
                    yield return (cell += side, ring);
            }
        }
    }
}
