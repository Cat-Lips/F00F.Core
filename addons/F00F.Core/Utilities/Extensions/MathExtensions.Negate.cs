namespace F00F;

public static class MathExtensions_Negate
{
    public static int Negate(this int source, bool negate = true) => negate ? -source : source;
    public static float Negate(this float source, bool negate = true) => negate ? -source : source;
}
