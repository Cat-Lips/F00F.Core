using System;
using Godot;

namespace F00F;

[Tool]
public partial class SlideBar : Root
{
    #region Private

    private Label Sep => field ??= (Label)GetNode("%Sep");
    private Button Title => field ??= (Button)GetNode("%Title");
    private Container Content => field ??= (Container)GetNode("%Content");

    #endregion

    #region Export

    [Export] public bool AutoOpen { get; set; }
    [Export] public string TitleText { get; set => this.Set(ref field, value, x => Title.Text = x); }
    [Export] public WidthGroup TitleWidth { get; set => this.Set(ref field, value, (o, n) => WidthGroup.Init(Title, o, n)); }
    [Export] public Color TitleForeColor { get; set => this.Set(ref field, value, x => Title.SetForeColor(x.NullIfDefault())); }
    [Export] public Color TitleBackColor { get; set => this.Set(ref field, value, x => Title.SetBackColor(x.NullIfDefault())); }

    public event Action IsOpenChanged;
    public bool IsOpen { get; private set => this.Set(ref field, value, OnIsOpenChanged, IsOpenChanged); }

    #endregion

    #region Private

    public SlideBar()
    {
        if (Editor.NotEditor)
            Ready += InitVisibility;

        InitVisibility();

        void InitVisibility()
        {
            SetMouseVisibility();
            OnVisibilityChanged();
            Title.Toggled += SetClickVisibility;
            MouseOverChanged += SetMouseVisibility;
            Content.VisibilityChanged += OnVisibilityChanged;

            void SetMouseVisibility()
            {
                if (AutoOpen)
                    IsOpen = MouseOver;
            }

            void SetClickVisibility(bool on)
            {
                if (!AutoOpen)
                    IsOpen = on;
            }

            void OnVisibilityChanged()
                => IsOpen = Content.Visible;
        }
    }

    private void OnIsOpenChanged()
    {
        Content.Visible = IsOpen;
        Title.ButtonPressed = IsOpen;
    }

    #endregion
}
