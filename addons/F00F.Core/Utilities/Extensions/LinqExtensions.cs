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

    public static TValue TryGet<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key)
        => source.TryGetValue(key, out var value) ? value : default;

    public static TValue TryGet<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue @default)
        => source.TryGetValue(key, out var value) ? value : @default;

    public static TValue TryGet<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, Func<TValue> GetDefault)
        => source.TryGetValue(key, out var value) ? value : GetDefault();

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
