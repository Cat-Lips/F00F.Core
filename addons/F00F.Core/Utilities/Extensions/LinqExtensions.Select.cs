using System;
using System.Collections.Generic;

namespace F00F;

public static class LinqExtensions_Select
{
    public static IEnumerable<T> SelectRecursive<T>(this T root, Func<T, IEnumerable<T>> select, bool self = false)
    {
        if (self) yield return root;

        foreach (var child in select(root))
        {
            yield return child;

            foreach (var sub in child.SelectRecursive(select))
                yield return sub;
        }
    }
}
