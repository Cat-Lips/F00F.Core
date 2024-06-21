using System;
using Godot;

namespace F00F
{
    public static class MathExtensions_Round
    {
        private const MidpointRounding DefaultRoundType = MidpointRounding.AwayFromZero;

        public static float Round(this float source, MidpointRounding type = DefaultRoundType) => source.Round(0, type);
        public static float Round(this float source, int digits, MidpointRounding type = DefaultRoundType) => float.Round(source, digits, type);

        public static Vector3 Round(this in Vector3 source, MidpointRounding type = DefaultRoundType) => source.Round(0, type);
        public static Vector3 Round(this in Vector3 source, int digits, MidpointRounding type = DefaultRoundType)
            => new(source.X.Round(digits, type), source.Y.Round(digits, type), source.Z.Round(digits, type));
    }
}
