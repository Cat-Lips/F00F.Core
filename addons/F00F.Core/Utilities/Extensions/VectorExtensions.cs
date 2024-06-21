using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace F00F
{
    public static class VectorExtensions
    {
        public static void Set(this ref Vector3 source, float? x = null, float? y = null, float? z = null)
            => source = new(x ?? source.X, y ?? source.Y, z ?? source.Z);

        public static Vector3 With(this in Vector3 source, float? x = null, float? y = null, float? z = null)
            => new(x ?? source.X, y ?? source.Y, z ?? source.Z);

        public static Vector2 XZ(this in Vector3 source) => new(source.X, source.Z);

        public static Vector3 Average<TSource>(this IEnumerable<TSource> source, Func<TSource, Vector3> selector) => source.Select(selector).Average();
        //public static Vector3 Average(this IEnumerable<Vector3> source) => new(source.Average(v => v.X), source.Average(v => v.Y), source.Average(v => v.Z));
        public static Vector3 Average(this IEnumerable<Vector3> source)
        {
            var count = 0;
            var sum = Vector3.Zero;
            foreach (var v in source)
            {
                ++count;
                sum += v;
            }

            return sum / count;
        }
    }
}
