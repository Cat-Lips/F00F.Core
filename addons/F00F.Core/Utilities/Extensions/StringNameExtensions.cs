using System.Collections.Generic;
using Godot;

namespace F00F;

public static class StringNameExtensions
{
    public static bool Equals(this StringName source, string value) => ((string)source).Equals(value);
    public static bool Contains(this StringName source, string value) => ((string)source).Contains(value);
    public static bool EndsWith(this StringName source, string value) => ((string)source).EndsWith(value);
    public static bool StartsWith(this StringName source, string value) => ((string)source).StartsWith(value);

    public static string Replace(this StringName source, string oldValue, string newValue) => ((string)source).Replace(oldValue, newValue);

    public static bool IsAnyOf(this StringName source, params IEnumerable<string> values) => ((string)source).IsAnyOf(values);
    public static bool IsNotAnyOf(this StringName source, params IEnumerable<string> values) => ((string)source).IsNotAnyOf(values);

    public static bool EqualsN(this StringName source, string value) => ((string)source).EqualsN(value);
    public static bool ContainsN(this StringName source, string value) => ((string)source).ContainsN(value);
    public static bool EndsWithN(this StringName source, string value) => ((string)source).EndsWithN(value);
    public static bool StartsWithN(this StringName source, string value) => ((string)source).StartsWithN(value);

    public static string ReplaceN(this StringName source, string oldValue, string newValue) => ((string)source).ReplaceN(oldValue, newValue);

    public static bool IsAnyOfN(this StringName source, params IEnumerable<string> values) => ((string)source).IsAnyOfN(values);
    public static bool IsNotAnyOfN(this StringName source, params IEnumerable<string> values) => ((string)source).IsNotAnyOfN(values);

    public static string Capitalise(this StringName source) => ((string)source).Capitalise();
    public static string ToCapitalCase(this StringName source) => ((string)source).ToCapitalCase();
    public static string SafeCapitalise(this StringName source) => ((string)source).SafeCapitalise();
    public static string ToSafeCapitalCase(this StringName source) => ((string)source).ToSafeCapitalCase();
}
