using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;

namespace F00F;

public static class Rng
{
    public static int Next() => Next(int.MaxValue);
    public static int Next(int max) => (int)(GD.Randi() % ((uint)max + 1));
    public static int Next(int min, int max) => Next(max - min) + min;

    public static float NextSingle() => GD.Randf();
    public static float NextSingle(float max) => NextSingle(0, max);
    public static float NextSingle(float min, float max) => (float)GD.RandRange(min, max);

    public static bool Evaluate(float probability)
        => probability >= NextSingle();

    public static T PickRandom<T>(params T[] array) => array[GD.Randi() % array.Length];
    public static T PickRandom<T>() where T : struct, Enum => PickRandom(Enum.GetValues<T>());
    public static T PickRandom<T>(params IList<T> array) => array[(int)(GD.Randi() % array.Count)];
    public static T PickRandom<T>(params IEnumerable<T> array) => array.OrderBy(x => GD.Randi()).FirstOrDefault();

    public static T PickWeighted<T>() where T : struct, Enum => PickWeighted(Enum.GetValues<T>());
    public static T PickWeighted<T>(params T[] array)
    {
        var idx = GD.Randi() % array.Length;
        var idxNormalised = idx / (float)(array.Length - 1);
        var bell = 4f * idxNormalised * (1f - idxNormalised);
        var idxWeighted = (bell * (array.Length - 1)).RoundInt();
        Debug.Assert(idxWeighted >= 0 && idxWeighted < array.Length);
        return array[idxWeighted];
    }
}
