using System;
using Godot;

namespace F00F
{
    [Tool]
    public partial class RootScroll : ScrollContainer
    {
        private Root _root;
        private Root Root => _root ??= Owner as Root;

        private MarginContainer Margin => GetNode<MarginContainer>("Margin");

        public override void _Ready()
        {
            InitMargin();
            InitContent();

            void InitMargin()
            {
                SetMargin();
                GetVScrollBar().VisibilityChanged += SetMargin;
                GetHScrollBar().VisibilityChanged += SetMargin;
            }

            void InitContent()
            {
                PerformLayout();
                Margin.MinimumSizeChanged += PerformLayout;
                GetViewport().SizeChanged += PerformLayout;
            }
        }

        private void SetMargin()
        {
            var rightMargin = ScrollSize(GetVScrollBar()).X;
            var bottomMargin = ScrollSize(GetHScrollBar()).Y;
            Margin.SetMargin(0, 0, RoundUp(rightMargin), RoundUp(bottomMargin));

            static int RoundUp(float f)
                => (int)MathF.Round(f, MidpointRounding.AwayFromZero);

            static Vector2 ScrollSize(ScrollBar scrollbar)
                => scrollbar.Visible ? scrollbar.Size : default;
        }

        private void PerformLayout()
        {
            if (Root is null) return;

            var usedMargin = Root.Size - Size;
            var allowedSize = Root.GetDisplayRect().Size - usedMargin;
            var requiredSize = Clamp(Margin.GetCombinedMinimumSize(), allowedSize);

            CustomMinimumSize = requiredSize;

            Root.CallDeferred(() => Root.SetAnchorsAndOffsetsPreset((LayoutPreset)Root.AnchorsPreset));

            static Vector2 Clamp(in Vector2 source, in Vector2 max)
                => new(Mathf.Min(source.X, max.X), Mathf.Min(source.Y, max.Y));
        }
    }
}
