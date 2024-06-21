using System;
using System.Runtime.CompilerServices;
using Godot;

namespace F00F;

using Range = (float? Min, float? Max, float? Step);

public static partial class UI
{
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

    public static SpinBox ValueEdit(int value, Action<int> OnEdit, Range range = default, string hint = null, float debounce = .1f, [CallerArgumentExpression(nameof(value))] string name = null) => _ValueEdit(name, value, x => OnEdit((int)x), @int: true, range, hint, debounce);
    public static SpinBox ValueEdit(float value, Action<float> OnEdit, Range range = default, string hint = null, float debounce = .1f, [CallerArgumentExpression(nameof(value))] string name = null) => _ValueEdit(name, value, x => OnEdit((float)x), @int: true, range, hint, debounce);
    public static SpinBox ValueEdit(string name, int value, Action<int> OnEdit, Range range = default, string hint = null, float debounce = .1f) => _ValueEdit(name, value, x => OnEdit((int)x), @int: false, range, hint, debounce);
    public static SpinBox ValueEdit(string name, float value, Action<float> OnEdit, Range range = default, string hint = null, float debounce = .1f) => _ValueEdit(name, value, x => OnEdit((float)x), @int: false, range, hint, debounce);
    private static SpinBox _ValueEdit(string name, double value, Action<double> OnEdit, bool @int, Range range, string hint, float debounce)
    {
        var timer = (SceneTreeTimer)null;
        var ec = NewValueEdit(name, hint, @int, range);
        ec.ValueChanged += OnValueChanged;
        ec.Value = value;
        return ec;

        void OnValueChanged(double _)
        {
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
}
