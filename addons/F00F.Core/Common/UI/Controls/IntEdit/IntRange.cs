using System;
using Godot;

namespace F00F;

[Tool]
public partial class IntRange : SpinBox
{
    public new event Action<int> ValueChanged;

    public new int Value
    {
        get => (int)base.Value;
        set => base.Value = value;
    }

    #region Private

    public IntRange()
    {
        base.Rounded = true;
        base.CustomArrowRound = true;
        base.ValueChanged += x => ValueChanged?.Invoke((int)x);
    }

    public new bool Rounded { get => base.Rounded = true; set => base.Rounded = true; }
    public new bool CustomArrowRound { get => base.CustomArrowRound = true; set => base.CustomArrowRound = true; }

    #endregion
}
