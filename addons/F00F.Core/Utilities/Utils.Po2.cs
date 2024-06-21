using Godot;

namespace F00F
{
    public static partial class Utils
    {
        public static bool IsPo2(int value)
            => value != 0 && (value & (value - 1)) == 0;

        public static int NearestLargerPo2(int value)
            => Mathf.NearestPo2(value);

        public static int NearestSmallerPo2(int value)
        {
            return IsPo2(value) ? value : NearestSmallerPo2(value);

            static int NearestSmallerPo2(int value)
            {
                value |= value >> 1;
                value |= value >> 2;
                value |= value >> 4;
                value |= value >> 8;
                value |= value >> 16;
                value -= value >> 1;
                return value;
            }
        }

        public static int NextPo2(int value, int from)
            => value < from ? NearestSmallerPo2(value) : NearestLargerPo2(value);
    }
}
