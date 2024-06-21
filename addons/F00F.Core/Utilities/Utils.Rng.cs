using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;

namespace F00F;

public static class Rng
{
    public static int Next() => GD.RandRange(0, int.MaxValue);
    public static float NextF() => GD.Randf();

    public static int Next(int max) => GD.RandRange(0, max);
    public static int Next(int min, int max) => GD.RandRange(min, max);
    public static float Next(float max) => (float)GD.RandRange(0, max);
    public static float Next(float min, float max) => (float)GD.RandRange(min, max);

    public static bool Evaluate(float probability)
        => NextF() < probability;

    public static T PickRandom<T>(params T[] array) => array[Next(array.Length - 1)];
    public static T PickRandom<T>() where T : struct, Enum => PickRandom(Enum.GetValues<T>());
    public static T PickRandom<T>(params IList<T> array) => array[Next(array.Count - 1)];
    public static T PickRandom<T>(params IEnumerable<T> array) => array.OrderBy(x => Next()).FirstOrDefault();

    public static T PickWeighted<T>() where T : struct, Enum => PickWeighted(Enum.GetValues<T>());
    public static T PickWeighted<T>(params T[] array)
    {
        var idx = Next(array.Length - 1);
        var idxNormalised = idx / (float)(array.Length - 1);
        var bell = 4f * idxNormalised * (1f - idxNormalised);
        var idxWeighted = (bell * (array.Length - 1)).RoundInt();
        Debug.Assert(idxWeighted >= 0 && idxWeighted < array.Length);
        return array[idxWeighted];
    }

    public static float NextWeighted(float limit)
    {
        var val = 0f;
        var dev = limit / 3;
        do val = (float)GD.Randfn(0f, dev);
        while (val < -limit || val > limit);
        return val;
    }

    public static Vector2 Vec2() => new(NextF(), NextF());
    public static Vector2 Vec2(float max) => new(Next(max), Next(max));
    public static Vector2 Vec2(float min, float max) => new(Next(min, max), Next(min, max));

    public static Vector3 Vec3() => new(NextF(), NextF(), NextF());
    public static Vector3 Vec3(float max) => new(Next(max), Next(max), Next(max));
    public static Vector3 Vec3(float min, float max) => new(Next(min, max), Next(min, max), Next(min, max));

    public static Vector2I Vec2I() => new(Next(), Next());
    public static Vector2I Vec2I(int max) => new(Next(max), Next(max));
    public static Vector2I Vec2I(int min, int max) => new(Next(min, max), Next(min, max));

    public static Vector3I Vec3I() => new(Next(), Next(), Next());
    public static Vector3I Vec3I(int max) => new(Next(max), Next(max), Next(max));
    public static Vector3I Vec3I(int min, int max) => new(Next(min, max), Next(min, max), Next(min, max));
}
