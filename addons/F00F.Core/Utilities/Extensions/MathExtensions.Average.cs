using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace F00F;

public static class MathExtensions_Average
{
    public static float AverageOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
        => source.Select(selector).AverageOrDefault();

    public static float AverageOrDefault(this IEnumerable<float> source)
        => source.DefaultIfEmpty().Average();

    public static Vector3 AverageOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, Vector3> selector)
        => source.Select(selector).AverageOrDefault();

    public static Vector3 AverageOrDefault(this IEnumerable<Vector3> source)
        => source.DefaultIfEmpty().Average();

    public static Vector3 Average<TSource>(this IEnumerable<TSource> source, Func<TSource, Vector3> selector)
        => source.Select(selector).Average();

    public static Vector3 Average(this IEnumerable<Vector3> source)
    {
        var count = 0;
        var sum = Vector3.Zero;

        foreach (var v in source)
        {
            sum += v;
            count++;
        }

        return count is 0 ? default : sum / count;
    }
}
