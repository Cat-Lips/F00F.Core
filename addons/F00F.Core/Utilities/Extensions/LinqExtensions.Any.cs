using System.Collections.Generic;
using System.Linq;

namespace F00F;

public static class LinqExtensions_Any
{
    public static bool IsAnyOf<T>(this T source, params IEnumerable<T> values)
        => values.Any(x => source.Equals(x));

    public static bool IsNotAnyOf<T>(this T source, params IEnumerable<T> values)
        => !values.Any(x => source.Equals(x));
}
