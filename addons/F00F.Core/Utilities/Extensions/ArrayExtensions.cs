using System.Collections.Generic;
using System.Linq;
using Godot;

namespace F00F;

public static class ArrayExtensions
{
    public static int Length<T>(this T[] source) => source?.Length ?? 0;
    public static bool IsEmpty<T>(this T[] source) => source?.Length is null or 0;
    public static bool NotEmpty<T>(this T[] source) => !source.IsEmpty();
    public static bool AnyNotNull<T>(this T[] source) => source?.Any(x => x is not null) ?? false;

    public static T[] Add<T>(this T[] source, T value) => [.. source, value];
    public static T[] Remove<T>(this T[] source, int idx) => source.Where((_, i) => i != idx).ToArray();
    public static T[] ShiftLeft<T>(this T[] source, int idx) => idx > 0 ? source.Swap(idx, idx - 1) : source;
    public static T[] ShiftRight<T>(this T[] source, int idx) => idx < source.Length - 1 ? source.Swap(idx, idx + 1) : source;
    public static T[] Swap<T>(this T[] source, int i1, int i2) { (source[i1], source[i2]) = (source[i2], source[i1]); return source; }

    public static T PickRandom<T>(this T[] source) => Rng.PickRandom(source);
    public static T PickWeighted<T>(this T[] source) => Rng.PickWeighted(source);
    public static T PickRandom<T>(this IEnumerable<T> source) => Rng.PickRandom(source);

    public static T PickNext<T>(this T[] source, ref int idx)
        => source[Mathf.PosMod(idx++, source.Length)];

    public static T GetOrAdd<T>(this IList<T> source, int idx) where T : new()
    {
        if (idx == source.Count)
            source.Add(new());
        return source[idx];
    }

    public static T[] NewIfNull<T>(this T[] source) where T : new()
    {
        source ??= [];
        for (var i = 0; i < source.Length; ++i)
            source[i] ??= new();
        return source;
    }
}
