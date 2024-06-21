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

    public static bool IsZero(this int x) => x is 0;
    public static bool IsZero(this float x) => x is 0;
    public static bool NotZero(this int x) => x is not 0;
    public static bool NotZero(this float x) => x is not 0;
}
