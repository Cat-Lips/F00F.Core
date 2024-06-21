using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Godot;

namespace F00F;

public static class EnumExtensions
{
    public static string Str<T>(this T source) where T : struct, Enum
        => $"{source}".Capitalise();

    public static string Trim<T>(this T source, string prefix) where T : struct, Enum
    {
        var str = $"{source}";
        if (prefix.NotNullOrEmpty())
            str = str.TrimPrefix(prefix);
        return str.Capitalise();
    }

    public static string Value<T>(this T source) where T : struct, Enum
    {
        var name = Enum.GetName(source);
        var member = typeof(T).GetMember(name)[0];
        var attr = (EnumMemberAttribute)Attribute.GetCustomAttribute(member, typeof(EnumMemberAttribute));
        return attr?.Value ?? name;
    }

    public static TEnum Min<TEnum>(this TEnum a, TEnum b) where TEnum : struct, Enum => Comparer<TEnum>.Default.Compare(a, b) <= 0 ? a : b;
    public static TEnum Max<TEnum>(this TEnum a, TEnum b) where TEnum : struct, Enum => Comparer<TEnum>.Default.Compare(a, b) >= 0 ? a : b;
    public static TEnum Clamp<TEnum>(this TEnum value, TEnum min, TEnum max) where TEnum : struct, Enum => value.Max(min).Min(max);

    public static int Index<T>(this T self) where T : struct, Enum
        => Array.IndexOf(Enum.GetValues<T>(), self);

    public static int Index<T>(this T? self) where T : struct, Enum
        => self is null ? -1 : self.Value.Index();

    #region Convert

    public static T AsEnum<T>(this int source) where T : struct, Enum
        => (T)Enum.ToObject(typeof(T), source);

    public static T AsEnum<T>(this long source) where T : struct, Enum
        => (T)Enum.ToObject(typeof(T), source);

    public static int AsInt<T>(this T source) where T : struct, Enum
        => Convert.ToInt32(source);

    public static long AsLong<T>(this T source) where T : struct, Enum
        => Convert.ToInt64(source);

    public static T AsEnum<T>(this string source) where T : struct, Enum
        => Enum.Parse<T>(source);

    public static string AsStr<T>(this T source) where T : struct, Enum
        => $"{source}";

    public static T[] AsEnumArray<T>(this string[] source) where T : struct, Enum
    {
        return [.. TryParseEnums()];

        IEnumerable<T> TryParseEnums()
        {
            foreach (var str in source)
            {
                if (Enum.TryParse<T>(str, out var value))
                    yield return value;
            }
        }
    }

    public static string[] AsStrArray<T>(this T[] source) where T : struct, Enum
        => [.. source.Select(x => x.AsStr())];

    #endregion
}
