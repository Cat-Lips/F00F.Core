using System;
using System.Collections.Generic;

namespace F00F;

public static class LinqExtensions_ForEach
{
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var item in source)
            action(item);
    }

    public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
    {
        var idx = -1;
        foreach (var item in source)
            action(item, ++idx);
    }

    public static void ForEach<T>(this ICollection<T> source, Action<T, int, float> action)
    {
        var idx = -1;
        var step = 1f / source.Count;
        foreach (var item in source)
            action(item, ++idx, step);
    }

    public static void ForEach(this int count, Action<int> action)
    {
        for (var i = 0; i < count; ++i)
            action(i);
    }

    public static void ForEach(this int count, Action action)
        => count.ForEach(_ => action());
}
