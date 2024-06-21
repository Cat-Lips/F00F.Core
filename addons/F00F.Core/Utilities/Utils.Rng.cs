using System;
using System.Diagnostics;
using Godot;

namespace F00F;

public static class Rng
{
    public static int Next() => Random.Shared.Next();
    public static int Next(int max) => Random.Shared.Next(max + 1);
    public static int Next(int min, int max) => Random.Shared.Next(min, max + 1);

    public static int Next(int min, int max, int step)
    {
        Debug.Assert(step > 0);
        var range = (max - min) / step;
        return min + Next(range) * step;
    }

    public static float NextSingle()
    {
        var value = Random.Shared.NextSingle(); // [0-1)
        return value < 1f ? value : 1; // [0-1]
    }

    public static float NextSingle(float max)
        => NextSingle() * max;

    public static float NextSingle(float min, float max)
        => min + NextSingle() * (max - min);

    public static float NextSingle(float min, float max, float step)
    {
        Debug.Assert(step > 0);
        var range = (max - min) / step;
        return min + NextSingle(range) * step;
    }

    public static bool NextBool(float probability)
        => Probability(NextSingle, probability);

    public static (int, int) MinMax(Func<int> next)
    {
        var (v1, v2) = (next(), next());
        return (Math.Min(v1, v2), Math.Max(v1, v2));
    }

    public static (float, float) MinMax(Func<float> next)
    {
        var (v1, v2) = (next(), next());
        return (Math.Min(v1, v2), Math.Max(v1, v2));
    }

    public static bool Probability(Func<float> next, float percent)
    {
        Debug.Assert(percent is >= 0 and <= 1);
        return next() < percent;
    }

    public static class Bell
    {
        private const int Size = 100;
        private static readonly float[] Values = new float[Size];

        static Bell()
        {
            for (var i = 0; i < Size; ++i)
            {
                var u1 = 1 - Rng.NextSingle();
                var u2 = 1 - Rng.NextSingle();
                Values[i] = Mathf.Sqrt(-2 * Mathf.Log(u1)) * Mathf.Sin(2 * Mathf.Pi * u2);
            }
        }

        public static int Next() => Next(int.MaxValue);
        public static int Next(int max) => NextSingle(max).RoundInt();
        public static int Next(int min, int max) => NextSingle(min, max).RoundInt();
        public static int Next(int min, int max, int step) => NextSingle(min, max, step).RoundInt();

        public static float NextSingle() => Values[Rng.Next(Size)];
        public static float NextSingle(float max) => NextSingle() * max;
        public static float NextSingle(float min, float max) => min + NextSingle() * (max - min);
        public static float NextSingle(float min, float max, float step)
        {
            Debug.Assert(step > 0);
            var range = (max - min) / step;
            return min + NextSingle(range) * step;
        }

        public static bool NextBool(float probability)
            => Probability(NextSingle, probability);
    }
}
