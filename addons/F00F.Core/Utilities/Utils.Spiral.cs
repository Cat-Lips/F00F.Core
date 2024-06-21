using System.Collections.Generic;
using Godot;

namespace F00F;

public static partial class Utils
{
    public static IEnumerable<(Vector2 Cell, int Ring)> Spiral(float x0, float y0, float radius) => Spiral(new(x0, y0), Mathf.CeilToInt(radius));
    public static IEnumerable<(Vector2 Cell, int Ring)> Spiral(float x0, float y0, int radius) => Spiral(new(x0, y0), radius);

    public static IEnumerable<(Vector2 Cell, int Ring)> Spiral(float radius) => Spiral(Vector2.Zero, Mathf.CeilToInt(radius));
    public static IEnumerable<(Vector2 Cell, int Ring)> Spiral(int radius) => Spiral(Vector2.Zero, radius);

    private static readonly Vector2[] Sides = [new(-1, 0), new(0, -1), new(1, 0), new(0, 1)];
    public static IEnumerable<(Vector2 Cell, int Ring)> Spiral(Vector2 origin, int radius)
    {
        if (radius < 0) yield break;

        yield return (origin, 0);

        for (var ring = 1; ring <= radius; ++ring)
        {
            var cell = new Vector2(origin.X + ring, origin.Y + ring);
            foreach (var side in Sides)
            {
                var count = 2 * ring;
                for (var cellIdx = 0; cellIdx < count; ++cellIdx)
                    yield return (cell += side, ring);
            }
        }
    }
}
