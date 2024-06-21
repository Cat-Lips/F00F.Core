using Godot;

namespace F00F;

public enum Sampling { Nearest, Bilinear, Bicubic }

public static class ImageDataExtensions
{
    #region GetData

    public static float[,] Data(this Image source) { source.Data(out var r, out var _); return r; }
    public static void Data(this Image source, out float[,] r, out Vector2I size) => source.Data(out r, out size.X, out size.Y);
    public static void Data(this Image source, out float[,] r, out float[,] g, out Vector2I size) => source.Data(out r, out g, out size.X, out size.Y);
    public static void Data(this Image source, out float[,] r, out float[,] g, out float[,] b, out Vector2I size) => source.Data(out r, out g, out b, out size.X, out size.Y);
    public static void Data(this Image source, out float[,] r, out float[,] g, out float[,] b, out float[,] a, out Vector2I size) => source.Data(out r, out g, out b, out a, out size.X, out size.Y);

    public static void Data(this Image source, out float[,] r, out int w, out int h)
    {
        w = source.GetWidth();
        h = source.GetHeight();
        r = new float[w, h];

        for (var x = 0; x < w; ++x)
        {
            for (var y = 0; y < h; ++y)
            {
                r[x, y] = source.GetPixel(x, y).R;
            }
        }
    }

    public static void Data(this Image source, out float[,] r, out float[,] g, out int w, out int h)
    {
        w = source.GetWidth();
        h = source.GetHeight();
        r = new float[w, h];
        g = new float[w, h];

        for (var x = 0; x < w; ++x)
        {
            for (var y = 0; y < h; ++y)
            {
                var p = source.GetPixel(x, y);
                r[x, y] = p.R;
                g[x, y] = p.G;
            }
        }
    }

    public static void Data(this Image source, out float[,] r, out float[,] g, out float[,] b, out int w, out int h)
    {
        w = source.GetWidth();
        h = source.GetHeight();
        r = new float[w, h];
        g = new float[w, h];
        b = new float[w, h];

        for (var x = 0; x < w; ++x)
        {
            for (var y = 0; y < h; ++y)
            {
                var p = source.GetPixel(x, y);
                r[x, y] = p.R;
                g[x, y] = p.G;
                b[x, y] = p.B;
            }
        }
    }

    public static void Data(this Image source, out float[,] r, out float[,] g, out float[,] b, out float[,] a, out int w, out int h)
    {
        w = source.GetWidth();
        h = source.GetHeight();
        r = new float[w, h];
        g = new float[w, h];
        b = new float[w, h];
        a = new float[w, h];

        for (var x = 0; x < w; ++x)
        {
            for (var y = 0; y < h; ++y)
            {
                var p = source.GetPixel(x, y);
                r[x, y] = p.R;
                g[x, y] = p.G;
                b[x, y] = p.B;
                a[x, y] = p.A;
            }
        }
    }

    #endregion

    #region GetValue

    public static float Value(this float[,] source, in Vector2 pos, Sampling mode = Sampling.Bilinear) => source.Value(pos.X, pos.Y);
    public static float Value(this float[,] source, float x, float y, Sampling mode = Sampling.Bilinear)
    {
        if (source is null) return default;

        var w = source.GetLength(0);
        var h = source.GetLength(1);

        x = Mathf.Clamp(x + w * .5f, 0, w - 1);
        y = Mathf.Clamp(y + h * .5f, 0, h - 1);

        var x0 = Mathf.FloorToInt(x);
        var y0 = Mathf.FloorToInt(y);

        var dx = x - x0;
        var dy = y - y0;

        return dx is 0 && dy is 0 ? source[x0, y0] : mode switch
        {
            Sampling.Nearest => Nearest(),
            Sampling.Bilinear => Bilinear(),
            Sampling.Bicubic => Bicubic(),
            _ => throw new System.NotImplementedException(),
        };

        float Nearest()
        {
            var xi = x.RoundInt().Clamp(0, w - 1);
            var yi = y.RoundInt().Clamp(0, h - 1);
            return source[xi, yi];
        }

        float Bilinear()
        {
            var x1 = (x0 + 1).ClampMax(w - 1);
            var y1 = (y0 + 1).ClampMax(h - 1);

            var p00 = source[x0, y0];
            var p01 = source[x0, y1];
            var p10 = source[x1, y0];
            var p11 = source[x1, y1];

            return
                p00 * (1 - dx) * (1 - dy) +
                p01 * (1 - dx) * dy +
                p10 * dx * (1 - dy) +
                p11 * dx * dy;
        }

        float Bicubic()
        {
            var x1 = (x0 + 1).ClampMax(w - 1);
            var x2 = (x0 + 2).ClampMax(w - 1);
            var xn1 = (x0 - 1).ClampMin(0);

            var y1 = (y0 + 1).ClampMax(h - 1);
            var y2 = (y0 + 2).ClampMax(h - 1);
            var yn1 = (y0 - 1).ClampMin(0);

            var p00 = source[x0, y0];
            var p01 = source[x0, y1];
            var p02 = source[x0, y2];
            var p10 = source[x1, y0];
            var p11 = source[x1, y1];
            var p12 = source[x1, y2];
            var p20 = source[x2, y0];
            var p21 = source[x2, y1];
            var p22 = source[x2, y2];

            var pn10 = source[xn1, y0];
            var pn11 = source[xn1, y1];
            var pn12 = source[xn1, y2];
            var p0n1 = source[x0, yn1];
            var p1n1 = source[x1, yn1];
            var p2n1 = source[x2, yn1];
            var pn1n1 = source[xn1, yn1];

            //         y-1        y0        y+1        y+2
            //       +---------+---------+---------+---------+
            // x-1   |  pn1n1  |  pn10   |  pn11   |  pn12   |
            //       +---------+---------+---------+---------+
            // x0    |  p0n1   |  p00    |  p01    |  p02    |
            //       +---------+---------+---------+---------+
            // x+1   |  p1n1   |  p10    |  p11    |  p12    |
            //       +---------+---------+---------+---------+
            // x+2   |  p2n1   |  p20    |  p21    |  p22    |
            //       +---------+---------+---------+---------+
            var r0 = CubicInterpolate(pn1n1, pn10, pn11, pn12, dx);
            var r1 = CubicInterpolate(p0n1, p00, p01, p02, dx);
            var r2 = CubicInterpolate(p1n1, p10, p11, p12, dx);
            var r3 = CubicInterpolate(p2n1, p20, p21, p22, dx);

            return CubicInterpolate(r0, r1, r2, r3, dy);

            float CubicInterpolate(float p0, float p1, float p2, float p3, float t)
            {
                // Catmull-Rom spline
                var a0 = -0.5f * p0 + 1.5f * p1 - 1.5f * p2 + 0.5f * p3;
                var a1 = p0 - 2.5f * p1 + 2f * p2 - 0.5f * p3;
                var a2 = -0.5f * p0 + 0.5f * p2;
                var a3 = p1;

                return ((a0 * t + a1) * t + a2) * t + a3;
            }
        }
    }

    #endregion
}
