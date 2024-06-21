using System;
using System.IO;
using System.Linq;
using Godot;

namespace F00F
{
    public static partial class StringExtensions
    {
        public static string Or(this string source, string dflt) => string.IsNullOrWhiteSpace(source) ? dflt : source;

        public static string CapLeft(this string source, int cap) => source.PadLeft(cap)[..cap];
        public static string CapRight(this string source, int cap) => source.PadRight(cap)[..cap];
        public static string AddPrefix(this string instance, string prefix) => instance.StartsWith(prefix) ? instance : $"{prefix}{instance}";
        public static string AddSuffix(this string instance, string suffix) => instance.EndsWith(suffix) ? instance : $"{instance}{suffix}";

        public static bool Contains(this string source, string value, bool cc = true) => source.Contains(value, cc ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
        public static bool Contains(this StringName source, string value, bool cc = true) => source.ToString().Contains(value, cc);
        public static bool ContainsN(this string source, string value) => source.Contains(value, cc: false);
        public static bool ContainsN(this StringName source, string value) => source.Contains(value, cc: false);

        public static string GetFileBaseName(this string source) => Path.GetFileNameWithoutExtension(source);

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
}
