using Godot;

namespace F00F;

[Tool]
public partial class QuickHelp : RootPopup
{
    private RichTextLabel Content => field ??= GetNode<RichTextLabel>("%Content");

    [Export(PropertyHint.File, "*.txt")]
    public string TextFile { get; set => this.Set(ref field, value, OnTextFileSet); }

    protected sealed override void OnReady()
    {
        LoadText();
        InitContent();

        void InitContent()
        {
            Content.MetaClicked += OpenUrl;

            static void OpenUrl(Variant link)
                => OS.ShellOpen((string)link);
        }
    }

    private void OnTextFileSet()
    {
        if (IsInsideTree())
            LoadText();
    }

    private void LoadText()
    {
        if (TextFile.IsNullOrEmpty()) { Content.Text = default; return; }

        var res = GD.Load(TextFile);
        if (res.IsNull()) { Content.Text = $"{TextFile} not found"; return; }

        // Content.Text = ???;
    }
}
