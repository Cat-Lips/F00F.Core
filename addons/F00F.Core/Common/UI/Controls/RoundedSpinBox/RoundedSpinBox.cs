using System;
using Godot;

namespace F00F;

public partial class RoundedSpinBox : SpinBox
{
    public new event Action<int> ValueChanged;

    public new int Value
    {
        get => (int)base.Value;
        set => base.Value = value;
    }

    #region Private

    public RoundedSpinBox()
    {
        base.Rounded = true;
        base.ValueChanged += OnValueChanged;

        void OnValueChanged(double x)
            => ValueChanged?.Invoke((int)x);
    }

    #endregion
}
