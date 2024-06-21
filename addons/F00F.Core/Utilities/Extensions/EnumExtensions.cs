using System;
using System.Runtime.Serialization;

namespace F00F;

public static class EnumExtensions
{
    public static string Str<T>(this T source) where T : struct, Enum
        => $"{source}".Capitalise();

    public static string Value<T>(this T source) where T : struct, Enum
    {
        var name = Enum.GetName(source);
        var member = typeof(T).GetMember(name)[0];
        var attr = (EnumMemberAttribute)Attribute.GetCustomAttribute(member, typeof(EnumMemberAttribute));
        return attr?.Value ?? name;
    }
}
