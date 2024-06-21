using System;
using System.Collections.Generic;

namespace F00F;

public static class LinqExtensions_Utils
{
    public static bool IsNull<T>(this T source) where T : class => source is null;
    public static bool NotNull<T>(this T source) where T : class => source is not null;

    public static IEnumerable<T> InterspersedWith<T>(this IEnumerable<T> source, T value)
        => source.InterspersedWith(() => value);

    public static IEnumerable<T> InterspersedWith<T>(this IEnumerable<T> source, Func<T> generator)
    {
        var first = true;
        foreach (var t in source)
        {
            if (!first) yield return generator();
            yield return t; first = false;
        }
    }
}
