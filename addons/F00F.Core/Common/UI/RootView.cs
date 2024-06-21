using System;
using Godot;

namespace F00F;

[Tool]
public partial class RootView : Root
{
    #region Private

    private Container Layout => field ??= (Container)GetNode("Panel/Margin/Layout");
    private Label TitleLabel => field ??= (Label)Layout.GetNode("Title");
    private Separator TitleSep => field ??= (Separator)Layout.GetNode("TitleSep");
    private ScrollContainer ScrollContainer => field ??= (ScrollContainer)Layout.GetNode("Scroll");
    private Container ContentContainer => field ??= (Container)ScrollContainer.GetNode("Margin");
    private Separator ButtonSep => field ??= (Separator)Layout.GetNode("ButtonSep");

    #endregion

    protected string TitleDefault { get; private set; }

    #region Export

    protected event Action TitleChanged;
    protected event Action ContentChanged;
    protected event Action ButtonsChanged;

    [Export] protected string Title { get; set => this.Set(ref field, value, TitleChanged); }
    [Export] protected PackedScene Content { get; set => this.Set(ref field, value, ContentChanged); }
    [Export] protected PackedScene Buttons { get; set => this.Set(ref field, value, ButtonsChanged); }

    #endregion

    public event Action ActiveChanged;
    public bool Active { get; private set => this.Set(ref field, value, ActiveChanged); }

    #region Godot

    protected sealed override void OnReady()
    {
        InitTitle();
        InitActive();
        InitContent();
        InitButtons();
    }

    #endregion

    #region Private

    private Node content = null;
    private Node buttons = null;

    private void ShowSelf()
    {
        var show = content.NotNull();
        Visible = show;
    }

    private void ShowTitle()
    {
        var show = Active && Title.NotNullOrEmpty();
        TitleLabel.Visible = show;
        TitleSep.Visible = show;
    }

    private void ShowContent()
    {
        var show = Active && content.NotNull();
        ScrollContainer.Visible = show;
    }

    private void ShowButtons()
    {
        var show = Active && buttons.NotNull();
        ButtonSep.Visible = show;
    }

    private void InitTitle()
    {
        TitleDefault = TitleLabel.Text;

        SetTitle(); ShowTitle();
        TitleChanged += SetTitle;
        TitleChanged += ShowTitle;

        void SetTitle()
            => TitleLabel.Text = Title;
    }

    private void InitActive()
    {
        OnActiveChanged();
        ActiveChanged += OnActiveChanged;

        void OnActiveChanged()
        {
            ShowTitle();
            ShowContent();
            ShowButtons();
        }
    }

    private void InitContent()
    {
        SetContent();
        ContentChanged += SetContent;

        void SetContent()
        {
            if (content.NotNull()) ContentContainer.FreeChild(content);
            content = Content?.Instantiate();
            if (content.NotNull()) ContentContainer.AddChild(content);

            ShowSelf();
            InitActive();
            ShowContent();

            void InitActive()
            {
                if (content is Container container)
                {
                    SetActive();
                    content.ChildEnteredTree += OnChildEntered;
                    content.ChildExitingTree += OnChildExiting;

                    void OnChildEntered(Node _) => SetActive();
                    void OnChildExiting(Node _) => this.CallDeferred(SetActive);
                    void SetActive() => Active = container.GetChildCount() > 0;
                }
                else Active = content.NotNull();
            }
        }
    }

    private void InitButtons()
    {
        SetButtons();
        ButtonsChanged += SetButtons;

        void SetButtons()
        {
            if (buttons.NotNull()) Layout.FreeChild(buttons);
            buttons = Buttons?.Instantiate();
            if (buttons.NotNull()) Layout.AddChild(buttons);

            ShowButtons();
        }
    }

    #endregion
}
