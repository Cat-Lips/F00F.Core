using Godot;

namespace F00F
{
    [Tool]
    public partial class Root : MarginContainer
    {
        private int _margin = 5;
        [Export] public int Margin { get => _margin; set => this.Set(ref _margin, Mathf.Max(value, 0), PerformLayout); }

        private MarginContainer OuterMargin => this;
        private MarginContainer InnerMargin => GetNode<MarginContainer>("%Margin");

        private void PerformLayout()
        {
            OuterMargin.SetMargin(Margin);
            InnerMargin.SetMargin(Margin);
        }

        public override void _Ready()
            => PerformLayout();
    }
}
