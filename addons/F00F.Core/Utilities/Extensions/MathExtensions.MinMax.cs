using System;
using System.Collections.Generic;
using System.Linq;

namespace F00F;

public static class MathExtensions_MinMax
{
    public static int MinOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector) => source.Select(selector).MinOrDefault();
    public static int MaxOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector) => source.Select(selector).MaxOrDefault();
    public static float MinOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector) => source.Select(selector).MinOrDefault();
    public static float MaxOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector) => source.Select(selector).MaxOrDefault();

    public static int MinOrDefault(this IEnumerable<int> source) => source.DefaultIfEmpty().Min();
    public static int MaxOrDefault(this IEnumerable<int> source) => source.DefaultIfEmpty().Max();
    public static float MinOrDefault(this IEnumerable<float> source) => source.DefaultIfEmpty().Min();
    public static float MaxOrDefault(this IEnumerable<float> source) => source.DefaultIfEmpty().Max();
}
