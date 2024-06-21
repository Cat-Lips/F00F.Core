using System;
using System.Collections.Generic;
using System.Linq;

namespace F00F;

public static class LinqExtensions_Any
{
    public static bool IsAnyOf<T>(this T source, params IEnumerable<T> values)
        => values.Any(x => source.Equals(x));

    public static bool IsNotAnyOf<T>(this T source, params IEnumerable<T> values)
        => !values.Any(x => source.Equals(x));

    public static R IfAny<T, R>(this IEnumerable<T> source, Func<IEnumerable<T>, R> action)
        => source.Any() ? action(source) : default;

    public static R IfAny<T, R>(this IEnumerable<T> source, Func<IEnumerable<T>, R> action, R dflt)
        => source.Any() ? action(source) : dflt;

    public static R IfAny<T, R>(this IEnumerable<T> source, Func<IEnumerable<T>, R> action, Func<R> dflt)
        => source.Any() ? action(source) : dflt();
}
