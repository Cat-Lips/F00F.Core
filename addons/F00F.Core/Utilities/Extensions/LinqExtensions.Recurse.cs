using System;
using System.Collections.Generic;

namespace F00F;

public static class LinqExtensions_Recurse
{
    public static IEnumerable<T> Recurse<T>(this T root, Func<T, IEnumerable<T>> select, bool self = false)
    {
        if (self) yield return root;

        foreach (var child in select(root))
        {
            foreach (var sub in child.Recurse(select, self: true))
                yield return sub;
        }
    }

    public static IEnumerable<T> RecurseWhile<T>(this T root, Func<T, IEnumerable<T>> select, Func<T, bool> predicate, bool self = false)
    {
        if (!predicate(root)) yield break;
        if (self) yield return root;

        foreach (var child in select(root))
        {
            foreach (var sub in child.RecurseWhile(select, predicate, self: true))
                yield return sub;
        }
    }
}
