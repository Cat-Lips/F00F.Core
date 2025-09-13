﻿using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace F00F;

public enum RGBA { R, G, B, A };

public static class ImageExtensions
{
    public static float SampleFloat(this Image source, in Vector2 size, in Vector2 offset, float x, float y, RGBA channel = RGBA.R, float dflt = 1)
    {
        if (source is null) return dflt;

        x = Mathf.PosMod(x + offset.X, size.X);
        y = Mathf.PosMod(y + offset.Y, size.Y);

        var pixel = source.InterpolatePixel(x, y);

        return channel switch
        {
            RGBA.R => pixel.R,
            RGBA.G => pixel.G,
            RGBA.B => pixel.B,
            RGBA.A => pixel.A,
            _ => throw new NotImplementedException($"{channel}"),
        };
    }

    public static Color SampleColor(this Image source, in Vector2 size, in Vector2 offset, float x, float y, float dflt = 1)
    {
        if (source is null) return new Color(dflt, dflt, dflt, dflt);

        x = Mathf.PosMod(x + offset.X, size.X);
        y = Mathf.PosMod(y + offset.Y, size.Y);

        return source.InterpolatePixel(x, y);
    }

    public static Color InterpolatePixel(this Image source, float x, float y)
    {
        var x0 = Mathf.FloorToInt(x);
        var y0 = Mathf.FloorToInt(y);

        var dx = x - x0;
        var dy = y - y0;

        if (dx is 0 && dy is 0)
            return source.GetPixel(x0, y0);

        var x1 = Mathf.CeilToInt(x);
        var y1 = Mathf.CeilToInt(y);

        var p00 = source.GetPixel(x0, y0);
        var p10 = source.GetPixel(x1, y0);
        var p01 = source.GetPixel(x0, y1);
        var p11 = source.GetPixel(x1, y1);

        return
            p00 * (1 - dx) * (1 - dy) +
            p10 * dx * (1 - dy) +
            p01 * (1 - dx) * dy +
            p11 * dx * dy;
    }

    public static Color AverageColor(this Image source)
    {
        var count = 0;
        float r = 0, g = 0, b = 0;

        foreach (var c in source.Colors())
        {
            r += c.R;
            g += c.G;
            b += c.B;
            ++count;
        }

        return new(r / count, g / count, b / count);
    }

    public static Color MostCommonColor(this Image source) => source.Colors()
            .GroupBy(x => x)
            .OrderByDescending(x => x.Count())
            .FirstOrDefault()?.Key ?? default;

    private static IEnumerable<Color> Colors(this Image source)
    {
        var bmp = new Bitmap();
        bmp.CreateFromImageAlpha(source);
        var size = bmp.GetSize();

        for (var x = 0; x < size.X; ++x)
        {
            for (var y = 0; y < size.Y; ++y)
            {
                if (bmp.GetBit(x, y))
                    yield return source.GetPixel(x, y);
            }
        }
    }

    public static int GetIndexOfFirstVisiblePixelFromTop(this Image source)
    {
        var bmp = new Bitmap();
        bmp.CreateFromImageAlpha(source);
        var size = bmp.GetSize();

        var x = size.X / 2;

        for (var y = 0; y < size.Y; ++y)
        {
            if (bmp.GetBit(x, y))
                return y;
        }

        return 0;
    }
}
