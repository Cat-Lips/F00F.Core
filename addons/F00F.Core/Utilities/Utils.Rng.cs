using System;
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

    public static T PickRandom<T>(params T[] array) => array[GD.Randi() % array.Length];
    public static T PickRandom<T>() where T : struct, Enum => PickRandom(Enum.GetValues<T>());
}
