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

    public event Action HasContentChanged;
    public bool HasContent { get; private set => this.Set(ref field, value, HasContentChanged); }

    #region Godot

    protected sealed override void OnReady()
    {
        if (Editor.NotEditor)
        {
            InitTitle();
            InitContent();
            InitButtons();
        }
    }

    #endregion

    #region Private

    private void InitTitle()
    {
        TitleDefault = TitleLabel.Text.NullIfEmpty();

        SetTitle(); ShowTitle();
        TitleChanged += SetTitle;
        TitleChanged += ShowTitle;
        HasContentChanged += ShowTitle;

        void SetTitle()
            => TitleLabel.Text = Title;

        void ShowTitle()
        {
            var show = HasContent && Title.NotNullOrEmpty();
            TitleLabel.Visible = show;
            TitleSep.Visible = show;
        }
    }

    private void InitContent()
    {
        Node content = null;
        InitContent(); ShowContent();
        ContentChanged += InitContent;
        HasContentChanged += ShowContent;

        void InitContent()
        {
            if (content.NotNull())
            {
                ContentContainer.FreeChild(content);
                (content as DataGrid)?.Empty -= SetHasContent;
            }

            content = Content?.Instantiate();

            if (content.NotNull())
            {
                ContentContainer.AddChild(content);
                (content as DataGrid)?.Empty += SetHasContent;
            }

            void SetHasContent()
                => HasContent = (content as DataGrid).HasRows;
        }

        void ShowContent()
            => ScrollContainer.Visible = HasContent;
    }

    private void InitButtons()
    {
        Node buttons = null;
        InitButtons(); ShowBtnSep();
        ButtonsChanged += InitButtons;
        HasContentChanged += ShowBtnSep;

        void InitButtons()
        {
            if (buttons.NotNull()) Layout.FreeChild(buttons);
            buttons = Buttons?.Instantiate();
            if (buttons.NotNull()) Layout.AddChild(buttons);
        }

        void ShowBtnSep()
            => ButtonSep.Visible = HasContent;
    }

    #endregion
}
