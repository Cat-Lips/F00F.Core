using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Godot;

namespace F00F;

using Range = (float? Min, float? Max, float? Step);

public static partial class UI
{
    public static OptionButton EnumEdit<T>(Action<T> OnSelect, T value, string hint = null, [CallerArgumentExpression(nameof(value))] string name = null) where T : struct, Enum => EnumEdit(name, value, OnSelect, hint);
    public static OptionButton EnumEdit<T>(T value, Action<T> OnSelect, string hint = null, [CallerArgumentExpression(nameof(value))] string name = null) where T : struct, Enum => EnumEdit(name, value, OnSelect, hint);
    public static OptionButton EnumEdit<T>(string name, T value, Action<T> OnSelect, string hint = null) where T : struct, Enum
    {
        var ec = NewOptionEdit();
        var names = Enum.GetNames<T>();
        var values = Enum.GetValues<T>();
        names.ForEach(x => ec.AddItem(x.Capitalise()));
        ec.ItemSelected += idx => OnSelect(values[idx]);
        ec.Selected = names.IndexOf($"{value}");
        return ec;

        OptionButton NewOptionEdit() => new()
        {
            Name = name,
            TooltipText = hint,
            FitToLongestItem = true,
        };
    }

    public static OptionButton OptionEdit(Action<string> OnSelect, string value, string[] items, string hint = null, [CallerArgumentExpression(nameof(value))] string name = null) => OptionEdit(name, value, OnSelect, items, hint);
    public static OptionButton OptionEdit(string value, Action<string> OnSelect, string[] items, string hint = null, [CallerArgumentExpression(nameof(value))] string name = null) => OptionEdit(name, value, OnSelect, items, hint);
    public static OptionButton OptionEdit(string name, string value, Action<string> OnSelect, string[] items, string hint = null)
    {
        var ec = NewOptionEdit();
        items.ForEach(x => ec.AddItem(x.Capitalise()));
        ec.ItemSelected += idx => OnSelect(items[idx]);
        ec.Selected = items.IndexOf(value);
        return ec;

        OptionButton NewOptionEdit() => new()
        {
            Name = name,
            TooltipText = hint,
            FitToLongestItem = true,
        };
    }

    public static SpinBox ValueEdit(int value, Action<int> OnEdit, Range range = default, string hint = null, float debounce = .1f, [CallerArgumentExpression(nameof(value))] string name = null) => ValueEdit(name, value, OnEdit, range, hint, debounce);
    public static SpinBox ValueEdit(float value, Action<float> OnEdit, Range range = default, string hint = null, float debounce = .1f, int? digits = null, [CallerArgumentExpression(nameof(value))] string name = null) => ValueEdit(name, value, OnEdit, range, hint, debounce, digits);
    public static SpinBox ValueEdit(string name, int value, Action<int> OnEdit, Range range = default, string hint = null, float debounce = .1f) => _ValueEdit(name, value, x => OnEdit((int)x), @int: false, range, hint, debounce);
    public static SpinBox ValueEdit(string name, float value, Action<float> OnEdit, Range range = default, string hint = null, float debounce = .1f, int? digits = null) => _ValueEdit(name, value, x => OnEdit((float)x), @int: false, range, hint, debounce, digits);
    private static SpinBox _ValueEdit(string name, double value, Action<double> OnEdit, bool @int, Range range, string hint, float debounce, int? digits = null)
    {
        var timer = (SceneTreeTimer)null;
        var ec = NewValueEdit(name, hint, @int, range);
        ec.ValueChanged += OnValueChanged;
        ec.Value = value;
        return ec;

        void OnValueChanged(double _)
        {
            if (digits.HasValue)
            {
                var newValue = ec.Value.Rounded(digits.Value);
                if (ec.Value != newValue)
                {
                    ec.Value = newValue;
                    return;
                }
            }

            if (TimerStarted())
                RestartTimer();
            else StartTimer();

            bool TimerStarted()
                => timer.NotNull();

            void RestartTimer()
                => timer.TimeLeft = debounce;

            void StartTimer()
            {
                timer = Root.CreateTimer(debounce);
                timer.Timeout += () =>
                {
                    timer = null;
                    OnEdit(ec.Value);
                };
            }
        }
    }

    public static Container ValueEdit(in Vector2 value, Action<Vector2> OnEdit, Range range = default, string hint = null, float debounce = .1f, int? digits = null, [CallerArgumentExpression(nameof(value))] string name = null) => ValueEdit(name, value, OnEdit, range, hint, debounce, digits);
    public static Container ValueEdit(in Vector3 value, Action<Vector3> OnEdit, Range range = default, string hint = null, float debounce = .1f, int? digits = null, [CallerArgumentExpression(nameof(value))] string name = null) => ValueEdit(name, value, OnEdit, range, hint, debounce, digits);
    public static Container ValueEdit(in Vector2I value, Action<Vector2I> OnEdit, Range range = default, string hint = null, float debounce = .1f, [CallerArgumentExpression(nameof(value))] string name = null) => ValueEdit(name, value, OnEdit, range, hint, debounce);
    public static Container ValueEdit(in Vector3I value, Action<Vector3I> OnEdit, Range range = default, string hint = null, float debounce = .1f, [CallerArgumentExpression(nameof(value))] string name = null) => ValueEdit(name, value, OnEdit, range, hint, debounce);
    public static Container ValueEdit(string name, in Vector2 value, Action<Vector2> OnEdit, Range range = default, string hint = null, float debounce = .1f, int? digits = null) => _MultiEdit(name, ["X", "Y"], [value.X, value.Y], x => OnEdit(new((float)x[0], (float)x[1])), @int: false, range, hint, debounce, digits);
    public static Container ValueEdit(string name, in Vector3 value, Action<Vector3> OnEdit, Range range = default, string hint = null, float debounce = .1f, int? digits = null) => _MultiEdit(name, ["X", "Y", "Z"], [value.X, value.Y, value.Z], x => OnEdit(new((float)x[0], (float)x[1], (float)x[2])), @int: false, range, hint, debounce, digits);
    public static Container ValueEdit(string name, in Vector2I value, Action<Vector2I> OnEdit, Range range = default, string hint = null, float debounce = .1f) => _MultiEdit(name, ["X", "Y"], [value.X, value.Y], x => OnEdit(new((int)x[0], (int)x[1])), @int: true, range, hint, debounce);
    public static Container ValueEdit(string name, in Vector3I value, Action<Vector3I> OnEdit, Range range = default, string hint = null, float debounce = .1f) => _MultiEdit(name, ["X", "Y", "Z"], [value.X, value.Y, value.Z], x => OnEdit(new((int)x[0], (int)x[1], (int)x[2])), @int: true, range, hint, debounce);
    private static Container _MultiEdit(string name, string[] parts, double[] values, Action<double[]> OnEdit, bool @int, Range range, string hint, float debounce, int? digits = null)
    {
        return Layout(name, Parts());

        IEnumerable<Control> Parts()
        {
            for (var idx = 0; idx < parts.Length; ++idx)
            {
                var i = idx;
                if (i > 0) yield return Sep(v: true);
                yield return _ValueEdit(parts[i], values[i], x => { values[i] = x; OnEdit(values); }, @int, range, hint, debounce, digits);
            }
        }
    }
}
