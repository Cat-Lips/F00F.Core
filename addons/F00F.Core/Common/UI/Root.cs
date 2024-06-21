using Godot;

namespace F00F;

[Tool]
public partial class Root : MarginContainer
{
    [Export] public int Margin { get; set => this.Set(ref field, Mathf.Max(value, 0), PerformLayout); } = 5;

    private MarginContainer OuterMargin => this;
    private MarginContainer InnerMargin => GetNode<MarginContainer>("%Margin");

    private void PerformLayout()
    {
        OuterMargin.SetMargin(Margin);
        InnerMargin.SetMargin(Margin);
    }

    protected virtual void OnReady() { }
    public sealed override void _Ready()
    {
        OnReady();
        PerformLayout();
    }
}
