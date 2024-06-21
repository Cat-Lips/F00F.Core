using System;
using System.Collections.Generic;
using System.Linq;

namespace F00F;

public static class MathExtensions_Average
{
    public static float AverageOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
        => source.Select(selector).AverageOrDefault();

    public static float AverageOrDefault(this IEnumerable<float> source)
    {
        var count = 0;
        var sum = 0f;

        foreach (var v in source)
        {
            sum += v;
            count++;
        }

        return count is 0 ? default : sum / count;
    }
}
