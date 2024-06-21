namespace F00F;

public static class MathExtensions_Po2
{
    public static bool IsPo2(this int value)
        => value >= 0 && (value & (value - 1)) is 0;

    public static int ToPo2(this int value, int from)
        => value.IsPo2() ? value : value < from ? PrevPo2(value) : NextPo2(value);

    public static int ToPo2(this int value)
    {
        --value;
        value |= value >> 1;
        value |= value >> 2;
        value |= value >> 4;
        value |= value >> 8;
        value |= value >> 16;
        return ++value;
    }

    public static int NextPo2(this int value)
    {
        value |= value >> 1;
        value |= value >> 2;
        value |= value >> 4;
        value |= value >> 8;
        value |= value >> 16;
        return ++value;
    }

    public static int PrevPo2(this int value)
    {
        --value;
        value |= value >> 1;
        value |= value >> 2;
        value |= value >> 4;
        value |= value >> 8;
        value |= value >> 16;
        value -= value >> 1;
        return value;
    }
}
