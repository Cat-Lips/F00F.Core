using System.Collections.Generic;
using Godot;

namespace F00F;

public static class StringNameExtensions
{
    public static bool EqualsN(this StringName source, string value) => ((string)source).EqualsN(value);
    public static bool ContainsN(this StringName source, string value) => ((string)source).ContainsN(value);
    public static bool EndsWithN(this StringName source, string value) => ((string)source).EndsWithN(value);

    public static string ReplaceN(this StringName source, string oldValue, string newValue) => ((string)source).ReplaceN(oldValue, newValue);

    public static bool IsAnyOfN(this StringName source, params IEnumerable<string> values) => ((string)source).IsAnyOfN(values);
    public static bool IsNotAnyOfN(this StringName source, params IEnumerable<string> values) => ((string)source).IsNotAnyOfN(values);

    public static string Capitalise(this StringName source) => ((string)source).Capitalise();
    public static string ToCapitalCase(this StringName source) => ((string)source).ToCapitalCase();
}
