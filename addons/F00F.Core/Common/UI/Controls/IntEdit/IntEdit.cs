using System;
using Godot;

namespace F00F;

[Tool]
public partial class IntEdit : RegEdit
{
    public event Action<int> ValueChanged;
    public int Value { get; set => this.Set(ref field, value, ValueChanged); }

    #region Private

    public IntEdit()
    {
        FilterPattern = "[^/d]";
        TextChanged += OnTextChanged;
        ValueChanged += OnValueChanged;

        void OnTextChanged(string text)
            => Value = text.TryInt() ?? Filter(text).TryInt() ?? default;

        void OnValueChanged(int value)
            => Text = $"{value}";
    }

    #endregion
}
