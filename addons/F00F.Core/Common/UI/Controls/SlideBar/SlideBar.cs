using System;
using Godot;

namespace F00F;

[Tool]
public partial class SlideBar : Container
{
    #region Private

    private Label Sep => field ??= (Label)GetNode(nameof(Sep));
    private Button Title => field ??= (Button)GetNode(nameof(Title));
    private Container Content => field ??= (Container)GetNode(nameof(Content));

    #endregion

    #region Export

    public event Action TitleTextChanged;
    public event Action TitleTextColorChanged;
    public event Action TitleBackColorChanged;

    [Export] public bool AutoOpen { get; set; }
    [Export] public string TitleText { get; set => this.Set(ref field, value, TitleTextChanged); }
    [Export] public WidthGroup TitleWidth { get; set => this.Set(ref field, value, (o, n) => WidthGroup.Init(Title, o, n)); }
    [Export] public Color TitleTextColor { get; set => this.Set(ref field, value, TitleTextColorChanged); }
    [Export] public Color TitleBackColor { get; set => this.Set(ref field, value, TitleBackColorChanged); }

    #endregion

    public event Action IsOpenChanged;
    public bool IsOpen { get; private set => this.Set(ref field, value, IsOpenChanged); }

    #region Godot

    protected virtual void OnReady() { }
    public sealed override void _Ready()
    {
        InitVisibility();
        InitTitle();
        OnReady();

        void InitTitle()
        {
            SetTitleText();
            SetTitleTextColor();
            SetTitleBackColor();

            TitleTextChanged += SetTitleText;
            TitleTextColorChanged += SetTitleTextColor;
            TitleBackColorChanged += SetTitleBackColor;

            void SetTitleText() => Title.Text = TitleText;
            void SetTitleTextColor() => Title.SetTextColor(TitleTextColor.NullIfDefault());
            void SetTitleBackColor() => Title.SetBackColor(TitleBackColor.NullIfDefault());
        }

        void InitVisibility()
        {
            SetVisibility();
            IsOpenChanged += SetVisibility;
            Title.Toggled += SetClickVisibility;
            this.InitMouseOver(SetMouseVisibility);
            Content.VisibilityChanged += OnVisibilityChanged;
            SetMouseVisibility(this.MouseOver());

            void SetVisibility()
            {
                Sep.Visible = IsOpen;
                Content.Visible = IsOpen;
                Title.ButtonPressed = IsOpen;
            }

            void OnVisibilityChanged()
                => IsOpen = Content.Visible;

            void SetClickVisibility(bool ToggleOn)
            {
                if (!AutoOpen)
                    IsOpen = ToggleOn;
            }

            void SetMouseVisibility(bool MouseOver)
            {
                if (AutoOpen)
                    IsOpen = MouseOver;
            }
        }
    }

    #endregion
}
