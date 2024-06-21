using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace F00F;

public static class StringExtensions
{
    public static int Length(this string source) => source?.Length ?? 0;
    public static bool IsEmpty(this string source) => source?.Length is null or 0;
    public static bool NotEmpty(this string source) => !source.IsEmpty();
    public static bool IsNullOrEmpty(this string source) => source is null or "";
    public static bool IsNullOrEmpty(this NodePath source) => source?.IsEmpty is null or true;
    public static bool IsNullOrEmpty(this StringName source) => source?.IsEmpty is null or true;

    public static string CapLeft(this string source, int cap) => source.PadLeft(cap)[..cap];
    public static string CapRight(this string source, int cap) => source.PadRight(cap)[..cap];
    public static string AddPrefix(this string instance, string prefix) => instance.StartsWith(prefix) ? instance : $"{prefix}{instance}";
    public static string AddSuffix(this string instance, string suffix) => instance.EndsWith(suffix) ? instance : $"{instance}{suffix}";

    public static bool EqualsN(this string source, string value) => source.Equals(value, StringComparison.OrdinalIgnoreCase);
    public static bool ContainsN(this string source, string value) => source.Contains(value, StringComparison.OrdinalIgnoreCase);
    public static bool EndsWithN(this string source, string value) => source.EndsWith(value, StringComparison.OrdinalIgnoreCase);
    public static bool StartsWithN(this string source, string value) => source.StartsWith(value, StringComparison.OrdinalIgnoreCase);

    public static string ReplaceN(this string source, string oldValue, string newValue) => source.Replace(oldValue, newValue, StringComparison.OrdinalIgnoreCase);

    public static bool IsAnyOfN(this string source, params IEnumerable<string> values) => values.Any(source.EqualsN);
    public static bool IsNotAnyOfN(this string source, params IEnumerable<string> values) => !values.Any(source.EqualsN);

    public static void SplitUrl(this string source, out string host, out string request)
    {
        var uri = new Uri(source);
        host = uri.GetLeftPart(UriPartial.Authority);
        request = uri.PathAndQuery;
    }

    public static void SplitCmd(this string source, out string exe, out string args)
    {
        var sep = source.StartsWith('"') ? '"' : ' ';
        var parts = Split(source, sep, 2);
        exe = parts.First();
        args = parts.Last();

        static string[] Split(string str, char sep, int count)
            => str.Split(sep, count, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    public static void SplitConnectStr(this string source, out string address, out int? port)
    {
        if (source.IsNullOrEmpty())
        {
            address = null;
            port = null;
            return;
        }

        var parts = source.Split(':');
        address = parts.First();
        port = parts.Skip(1).SingleOrDefault().TryInt();
    }

    public static string Capitalise(this string source) => source.Capitalize();
    public static string ToCapitalCase(this string source) => source.ToPascalCase();

    public static string SafeCapitalise(this string source) => source.Replace("-", "_").Capitalize();
    public static string ToSafeCapitalCase(this string source) => source.Replace("-", "_").ToPascalCase();

    public static string Pluralise(this string source, bool @if = true, string plural = null)
        => @if ? plural is null ? $"{source}s" : plural : source;

    public static string Prefix(this string source, string prefix = null)
        => source is null ? null : $"{prefix}{source}";

    #region Conversions

    public static bool IsInt(this string source) => source.IsInt(out var _);
    public static bool IsFloat(this string source) => source.IsFloat(out var _);
    public static bool IsVec2(this string source, char sep = ',') => source.IsVec2(out var _, sep);
    public static bool IsVec2I(this string source, char sep = ',') => source.IsVec2I(out var _, sep);
    public static bool IsVec3(this string source, char sep = ',') => source.IsVec3(out var _, sep);
    public static bool IsVec3I(this string source, char sep = ',') => source.IsVec3I(out var _, sep);

    public static int ToInt(this string source, int dflt) => source.TryInt() ?? dflt;
    public static float ToFloat(this string source, float dflt) => source.TryFloat() ?? dflt;
    public static Vector2 ToVec2(this string source, in Vector2 dflt, char sep = ',') => source.TryVec2(sep) ?? dflt;
    public static Vector2I ToVec2I(this string source, in Vector2I dflt, char sep = ',') => source.TryVec2I(sep) ?? dflt;
    public static Vector3 ToVec3(this string source, in Vector3 dflt, char sep = ',') => source.TryVec3(sep) ?? dflt;
    public static Vector3I ToVec3I(this string source, in Vector3I dflt, char sep = ',') => source.TryVec3I(sep) ?? dflt;

    public static int? TryInt(this string source) => source.IsInt(out var value) ? value : null;
    public static float? TryFloat(this string source) => source.IsFloat(out var value) ? value : null;
    public static Vector2? TryVec2(this string source, char sep = ',') => source.IsVec2(out var value, sep) ? value : null;
    public static Vector2I? TryVec2I(this string source, char sep = ',') => source.IsVec2I(out var value, sep) ? value : null;
    public static Vector3? TryVec3(this string source, char sep = ',') => source.IsVec3(out var value, sep) ? value : null;
    public static Vector3I? TryVec3I(this string source, char sep = ',') => source.IsVec3I(out var value, sep) ? value : null;

    public static bool IsInt(this string source, out int value) => int.TryParse(source, out value);
    public static bool IsFloat(this string source, out float value) => float.TryParse(source, out value);

    public static bool IsVec2(this string source, out Vector2 value, char sep = ',')
    {
        var parts = source?.Split(sep);
        if (parts.Length() is 2 && parts[0].IsFloat(out var x) && parts[1].IsFloat(out var y))
        {
            value = new(x, y);
            return true;
        }

        value = default;
        return false;
    }

    public static bool IsVec2I(this string source, out Vector2I value, char sep = ',')
    {
        var parts = source?.Split(sep);
        if (parts.Length() is 2 && parts[0].IsInt(out var x) && parts[1].IsInt(out var y))
        {
            value = new(x, y);
            return true;
        }

        value = default;
        return false;
    }

    public static bool IsVec3(this string source, out Vector3 value, char sep = ',')
    {
        var parts = source?.Split(sep);
        if (parts.Length() is 3 && parts[0].IsFloat(out var x) && parts[1].IsFloat(out var y) && parts[2].IsFloat(out var z))
        {
            value = new(x, y, z);
            return true;
        }

        value = default;
        return false;
    }

    public static bool IsVec3I(this string source, out Vector3I value, char sep = ',')
    {
        var parts = source?.Split(sep);
        if (parts.Length() is 3 && parts[0].IsInt(out var x) && parts[1].IsInt(out var y) && parts[2].IsInt(out var z))
        {
            value = new(x, y, z);
            return true;
        }

        value = default;
        return false;
    }

    #endregion
}
