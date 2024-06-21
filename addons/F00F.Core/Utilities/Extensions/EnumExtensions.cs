using System;
using System.Runtime.Serialization;
using Godot;

namespace F00F;

public static class EnumExtensions
{
    public static string Str<T>(this T source, string trimPrefix = null) where T : struct, Enum
    {
        var str = $"{source}";
        if (trimPrefix.NotNullOrEmpty())
            str = str.TrimPrefix(trimPrefix);
        return str.Capitalise();
    }

    public static string Value<T>(this T source) where T : struct, Enum
    {
        var name = Enum.GetName(source);
        var member = typeof(T).GetMember(name)[0];
        var attr = (EnumMemberAttribute)Attribute.GetCustomAttribute(member, typeof(EnumMemberAttribute));
        return attr?.Value ?? name;
    }

    public static string Humanise<T>(this T source, string trimPrefix = null) where T : struct, Enum => source.Str(trimPrefix);
    public static string Capitalise<T>(this T source, string trimPrefix = null) where T : struct, Enum => source.Str(trimPrefix);
}
