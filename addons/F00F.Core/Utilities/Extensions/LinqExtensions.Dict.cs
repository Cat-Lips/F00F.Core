using System;
using System.Collections.Generic;
using System.Linq;

namespace F00F;

public static class LinqExtensions_Dict
{
    public static TValue TryGet<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key)
        => source.TryGetValue(key, out var value) ? value : default;

    public static TValue TryGet<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue dflt)
        => source.TryGetValue(key, out var value) ? value : dflt;

    public static TValue TryGet<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, Func<TValue> GetDefault)
        => source.TryGetValue(key, out var value) ? value : GetDefault();

    public static bool TrySet<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue value)
    {
        return source.TryGetValue(key, out var old) ? UpdateValue() : AddValue();

        bool UpdateValue()
        {
            if (Equals(old, value))
                return false;

            source[key] = value;
            return true;
        }

        bool AddValue()
        {
            source.Add(key, value);
            return true;
        }
    }

    public static void TrySet<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue value, Action<TKey, TValue> OnChanged)
    {
        if (source.TrySet(key, value))
            OnChanged?.Invoke(key, value);
    }

    public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key) => source.TryGet(key);
    public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue dflt) => source.TryGet(key, dflt);
    public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, Func<TValue> GetDefault) => source.TryGet(key, GetDefault);
    public static bool Set<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue value) => source.TrySet(key, value);
    public static void Set<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue value, Action<TKey, TValue> OnChanged) => source.TrySet(key, value, OnChanged);

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

    public static IDictionary<TKey, TValue> Copy<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> source)
        => source.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
}
