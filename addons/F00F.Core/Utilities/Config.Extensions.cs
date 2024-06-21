using System;
using System.Runtime.CompilerServices;
using Godot;

namespace F00F;

public static class ConfigExtensions
{
    public static void Set<[MustBeVariant] T>(this Config source, T value, [CallerArgumentExpression(nameof(value))] string key = null) => source.Set(key, value);
    public static void Set<[MustBeVariant] T>(this Config source, string section, T value, [CallerArgumentExpression(nameof(value))] string key = null) => source.Set(section, key, value);

    public static T Get<[MustBeVariant] T>(this Config source, T dflt, [CallerArgumentExpression(nameof(dflt))] string key = null) => source.Get(key, dflt);
    public static T Get<[MustBeVariant] T>(this Config source, string section, T dflt, [CallerArgumentExpression(nameof(dflt))] string key = null) => source.Get(section, key, dflt);

    #region Enums

    public static void SetEnum<T>(this Config source, string key, T value) where T : struct, Enum => source.Set(key, value.AsStr());
    public static void SetEnum<T>(this Config source, string section, string key, T value) where T : struct, Enum => source.Set(section, key, value.AsStr());

    public static T? GetEnum<T>(this Config source, string key) where T : struct, Enum => source.TryGetEnum<T>(key, out var value) ? value : null;
    public static T? GetEnum<T>(this Config source, string section, string key) where T : struct, Enum => source.TryGetEnum<T>(section, key, out var value) ? value : null;

    public static T GetEnum<T>(this Config source, string key, T dflt) where T : struct, Enum => source.TryGetEnum<T>(key, out var value) ? value : dflt;
    public static T GetEnum<T>(this Config source, string section, string key, T dflt) where T : struct, Enum => source.TryGetEnum<T>(section, key, out var value) ? value : dflt;

    public static bool TryGetEnum<T>(this Config source, string key, out T value) where T : struct, Enum => source.TryGetEnum("", key, out value);
    public static bool TryGetEnum<T>(this Config source, string section, string key, out T value) where T : struct, Enum
    {
        if (source.TryGet<string>(section, key, out var str))
        {
            value = str.AsEnum<T>();
            return true;
        }

        value = default;
        return false;
    }

    #endregion

    #region Enum[]

    public static void SetEnums<T>(this Config source, string key, T[] value) where T : struct, Enum => source.Set(key, value.AsStrArray());
    public static void SetEnums<T>(this Config source, string section, string key, T[] value) where T : struct, Enum => source.Set(section, key, value.AsStrArray());

    public static T[] GetEnums<T>(this Config source, string key) where T : struct, Enum => source.TryGetEnums<T>(key, out var value) ? value : null;
    public static T[] GetEnums<T>(this Config source, string section, string key) where T : struct, Enum => source.TryGetEnums<T>(section, key, out var value) ? value : null;

    public static T[] GetEnums<T>(this Config source, string key, T[] dflt) where T : struct, Enum => source.TryGetEnums<T>(key, out var value) ? value : dflt;
    public static T[] GetEnums<T>(this Config source, string section, string key, T[] dflt) where T : struct, Enum => source.TryGetEnums<T>(section, key, out var value) ? value : dflt;

    public static bool TryGetEnums<T>(this Config source, string key, out T[] value) where T : struct, Enum => source.TryGetEnums("", key, out value);
    public static bool TryGetEnums<T>(this Config source, string section, string key, out T[] value) where T : struct, Enum
    {
        if (source.TryGet<string[]>(section, key, out var strArray))
        {
            value = strArray.AsEnumArray<T>();
            return true;
        }

        value = default;
        return false;
    }

    #endregion

    #region Nodes

    public static void Set<[MustBeVariant] T>(this Config source, Node key, T value) => source.Set(key.Name, value);
    public static void Set<[MustBeVariant] T>(this Config source, Node section, Node key, T value) => source.Set(section.Name, key.Name, value);

    public static T Get<[MustBeVariant] T>(this Config source, Node key) => source.Get<T>(key.Name);
    public static T Get<[MustBeVariant] T>(this Config source, Node section, Node key) => source.Get<T>(section.Name, key.Name);

    public static T Get<[MustBeVariant] T>(this Config source, Node key, T dflt) => source.Get(key.Name, dflt);
    public static T Get<[MustBeVariant] T>(this Config source, Node section, Node key, T dflt) => source.Get(section.Name, key.Name, dflt);

    public static bool TryGet<[MustBeVariant] T>(this Config source, Node key, out T value) => source.TryGet(key.Name, out value);
    public static bool TryGet<[MustBeVariant] T>(this Config source, Node section, Node key, out T value) => source.TryGet(section.Name, key.Name, out value);

    public static void Clear(this Config source, Node key) => source.Clear(key.Name);
    public static void Clear(this Config source, Node section, Node key) => source.Clear(section.Name, key.Name);

    #endregion
}
