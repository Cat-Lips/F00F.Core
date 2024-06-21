using System;
using System.Collections.Generic;
using System.Linq;

namespace F00F;

public static class LinqExtensions
{
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var item in source)
            action(item);
    }

    public static IEnumerable<T> SelectRecursive<T>(this T root, Func<T, IEnumerable<T>> select, bool self = false)
    {
        if (self)
            yield return root;

        foreach (var child in select(root))
        {
            yield return child;

            foreach (var sub in child.SelectRecursive(select))
                yield return sub;
        }
    }

    public static bool IsAnyOf<T>(this T source, params IEnumerable<T> values)
        => values.Any(x => source.Equals(x));

    public static bool IsNotAnyOf<T>(this T source, params IEnumerable<T> values)
        => !values.Any(x => source.Equals(x));

    public static R IfAny<T, R>(this IEnumerable<T> source, Func<IEnumerable<T>, R> action)
        => source.Any() ? action(source) : default;

    public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, Func<TValue> GetDefault = null)
        => source.TryGetValue(key, out var value) ? value : GetDefault is not null ? GetDefault() : default;

    public static IEnumerable<(TKey Key, TValue Value)> OrderBy<TKey, TValue>(this IDictionary<TKey, TValue> source, IEnumerable<TKey> keys)
    {
        foreach (var key in keys)
        {
            if (source.TryGetValue(key, out var value))
                yield return (key, value);
        }

        var lookup = new HashSet<TKey>(keys);
        foreach (var (key, value) in source)
        {
            if (!lookup.Contains(key))
                yield return (key, value);
        }
    }
}
