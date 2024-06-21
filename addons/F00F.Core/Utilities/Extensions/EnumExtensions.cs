using System;
using System.Collections.Generic;
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
}
