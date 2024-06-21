using System.Globalization;
using System.Linq;

namespace F00F;

public static class MathExtensions_Helpers
{
    public static int Negate(this int source, bool negate = true) => negate ? -source : source;
    public static float Negate(this float source, bool negate = true) => negate ? -source : source;

    public static int CountDecimalPlaces(this float source)
    {
        var str = source.ToString(CultureInfo.InvariantCulture);
        var idx = str.IndexOf('.');
        return idx is -1 ? 0 : str.Length - idx - 1;
    }

    public static float GetMostRoundValue(this float[] source)
        => source.Select(x => (Value: x, Count: x.CountDecimalPlaces())).MinBy(x => x.Count).Value;
}
