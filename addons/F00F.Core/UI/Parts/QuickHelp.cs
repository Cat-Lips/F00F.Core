using Godot;

namespace F00F;

[Tool]
public partial class QuickHelp : RootPopup
{
    private RichTextLabel Content => field ??= (RichTextLabel)GetNode("%Content");

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
        if (IsNodeReady())
            LoadText();
    }

    private void LoadText()
    {
        Content.Text = TextFile.IsNullOrEmpty() ? DefaultText() : LoadedText();

        string DefaultText()
            => MyInput.TabulateBB();

        string LoadedText()
        {
            var help = FS.FileExists(TextFile)
                ? FS.ReadFile(TextFile)
                : $"Could not find file {TextFile}\n";

            return string.Join("\n\n", [help, MyInput.TabulateBB()]).Trim();
        }
    }
}
