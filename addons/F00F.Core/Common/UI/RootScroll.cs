using Godot;

namespace F00F;

[Tool]
public partial class RootScroll : ScrollContainer
{
    private Control Root => field ??= Owner as Control ?? this.GetRoot();

    private MarginContainer Margin => field ??= (MarginContainer)GetNode("Margin");

    public sealed override void _Ready()
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
        Margin.SetMargin(0, 0, rightMargin.RoundInt(), bottomMargin.RoundInt());

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

        if (Root.AnchorsPreset is not -1)
            Root.CallDeferred(() => Root.SetAnchorsAndOffsetsPreset((LayoutPreset)Root.AnchorsPreset));

        static Vector2 Clamp(in Vector2 source, in Vector2 max)
            => new(Mathf.Min(source.X, max.X), Mathf.Min(source.Y, max.Y));
    }
}
