using System.Collections.Generic;
using Godot;

namespace F00F
{
    public static partial class Utils
    {
        public static IEnumerable<(Vector2 Cell, int Ring)> Spiral(in Vector3 origin, float radius) => Spiral(origin.X, origin.Z, Mathf.CeilToInt(radius));
        public static IEnumerable<(Vector2 Cell, int Ring)> Spiral(in Vector3 origin, int radius) => Spiral(origin.X, origin.Z, radius);

        public static IEnumerable<(Vector2 Cell, int Ring)> Spiral(float x0, float y0, float radius) => Spiral(new(x0, y0), Mathf.CeilToInt(radius));
        public static IEnumerable<(Vector2 Cell, int Ring)> Spiral(float x0, float y0, int radius) => Spiral(new(x0, y0), radius);

        public static IEnumerable<(Vector2 Cell, int Ring)> Spiral(float radius) => Spiral(Vector2.Zero, Mathf.CeilToInt(radius));
        public static IEnumerable<(Vector2 Cell, int Ring)> Spiral(int radius) => Spiral(Vector2.Zero, radius);

        public static IEnumerable<(Vector2 Cell, int Ring)> Spiral(Vector2 origin, int radius)
        {
            var x = 0;
            var y = 0;
            var leg = 0;
            var layer = 1;

            if (radius is 0)
                yield break;

            yield return (origin, 0);
            var absOrigin = origin.Abs();

            while (true)
            {
                var next = Next();
                if (x == radius) break;
                yield return (next, Ring(next));
            }

            Vector2 Next()
            {
                switch (leg)
                {
                    case 0:
                        if (++x == layer) ++leg;
                        break;
                    case 1:
                        if (++y == layer) ++leg;
                        break;
                    case 2:
                        if (--x == -layer) ++leg;
                        break;
                    case 3:
                        if (--y == -layer) { leg = 0; ++layer; }
                        break;
                }

                return new(origin.X + x, origin.Y + y);
            }

            int Ring(in Vector2 cell)
            {
                var diff = cell.Abs() - absOrigin;
                var max = Mathf.Max(diff.X, diff.Y);
                return (int)max.Round();
            }
        }
    }
}
