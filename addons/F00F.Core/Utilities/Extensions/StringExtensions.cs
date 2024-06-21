using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace F00F;

public static partial class StringExtensions
{
    public static string Or(this string source, string dflt) => string.IsNullOrWhiteSpace(source) ? dflt : source;

    public static string CapLeft(this string source, int cap) => source.PadLeft(cap)[..cap];
    public static string CapRight(this string source, int cap) => source.PadRight(cap)[..cap];
    public static string AddPrefix(this string instance, string prefix) => instance.StartsWith(prefix) ? instance : $"{prefix}{instance}";
    public static string AddSuffix(this string instance, string suffix) => instance.EndsWith(suffix) ? instance : $"{instance}{suffix}";

    public static bool EqualsN(this string source, string value) => string.Equals(source, value, StringComparison.OrdinalIgnoreCase);
    public static bool ContainsN(this string source, string value) => source.Contains(value, StringComparison.OrdinalIgnoreCase);
    public static bool EndsWithN(this string source, string value) => source.EndsWith(value, StringComparison.OrdinalIgnoreCase);

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

    public static string Capitalise(this string source)
        => source.Capitalize();

    //public static string Capitalise(this string source)
    //    => SplitStringAtCase().Replace(source, " $1");

    //[GeneratedRegex(@"([A-Z][a-z]+|[A-Z]+[A-Z]|[A-Z]|[^A-Za-z]+[^A-Za-z]|2D|3D)", RegexOptions.RightToLeft | RegexOptions.Compiled)]
    //private static partial Regex SplitStringAtCase();
}
