using System;
using Godot;

namespace F00F
{
    public static class MathExtensions_Round
    {
        private const MidpointRounding DefaultRoundType = MidpointRounding.AwayFromZero;

        public static float Round(this float source, MidpointRounding type = DefaultRoundType) => source.Round(0, type);
        public static float Round(this float source, int digits, MidpointRounding type = DefaultRoundType)
            => float.Round(source, digits, type);

        public static Vector3 Round(this in Vector3 source, float? x = null, float? y = null, float? z = null, MidpointRounding type = DefaultRoundType)
            => new(x ?? source.X.Round(type), y ?? source.Y.Round(type), z ?? source.Z.Round(type));

        public static Vector3 RoundXZ(this in Vector3 source, float y = 0, MidpointRounding type = DefaultRoundType)
            => source.Round(y: y, type: type);

        public static Vector3 RoundXYZ(this in Vector3 source, MidpointRounding type = DefaultRoundType)
            => source.Round(type: type);
    }
}
