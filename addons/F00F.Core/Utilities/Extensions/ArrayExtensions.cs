using System;
using System.Collections.Generic;
using System.Linq;

namespace F00F
{
    public static class ArrayExtensions
    {
        public static int Length<T>(this T[] source) => source?.Length ?? 0;
        public static bool Empty<T>(this T[] source) => source?.Length is null or 0;

        public static T[] Add<T>(this T[] source, T value) => [.. source, value];
        public static T[] Remove<T>(this T[] source, int idx) => source.Where((_, i) => i != idx).ToArray();
        public static T[] ShiftLeft<T>(this T[] source, int idx) => idx > 0 ? source.Swap(idx, idx - 1) : source;
        public static T[] ShiftRight<T>(this T[] source, int idx) => idx < source.Length - 1 ? source.Swap(idx, idx + 1) : source;
        public static T[] Swap<T>(this T[] source, int i1, int i2) { (source[i1], source[i2]) = (source[i2], source[i1]); return source; }

        public static T GetRandom<T>(this T[] source) => source[Random.Shared.Next() % source.Length];

        public static T GetOrAdd<T>(this IList<T> source, int idx) where T : new()
        {
            if (idx == source.Count)
                source.Add(new());
            return source[idx];
        }
    }
}
