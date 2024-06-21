using Godot;

namespace F00F;

[Tool]
public partial class SuperSlider : Container
{
    #region Private

    private Button BtnMin => field ??= IsNodeReady() ? (Button)GetNode("%Min") : null;
    private Button BtnMax => field ??= IsNodeReady() ? (Button)GetNode("%Max") : null;
    private Slider Slider => field ??= IsNodeReady() ? (Slider)GetNode("%Slider") : null;

    #endregion

    #region Export

    [Export] public string MinText { get; set => this.Set(ref field, value, SetMinText); }
    [Export] public string MaxText { get; set => this.Set(ref field, value, SetMaxText); }
    [Export] public WidthGroup MinGroup { get; set => this.Set(ref field, value, (o, n) => SetGroup(BtnMin, o, n)); }
    [Export] public WidthGroup MaxGroup { get; set => this.Set(ref field, value, (o, n) => SetGroup(BtnMax, o, n)); }
    [Export] public int Value { get; set => this.Set(ref field, value, SetSliderValue); }

    #endregion

    #region Godot

    public sealed override void _Ready()
    {
        SetMinText();
        SetMaxText();
        SetSliderValue();
        MinGroup?.Add(BtnMin);
        MaxGroup?.Add(BtnMax);
    }

    #endregion

    #region Private

    private void SetMinText() => BtnMin?.Text = MinText;
    private void SetMaxText() => BtnMax?.Text = MaxText;
    private void SetSliderValue() => Slider?.Value = Value;

    private static void SetGroup(Control x, WidthGroup old, WidthGroup @new)
    {
        old?.Remove(x);
        @new?.Add(x);
    }

    #endregion
}
