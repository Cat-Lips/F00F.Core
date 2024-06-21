#if TOOLS
using Godot;

namespace F00F;

public partial class QuickHelp
{
    protected sealed override void OnEditorSave()
    {
        Editor.DoPreSaveReset(this, PropertyName.TextFile);
        Editor.DoPreSaveReset(Content, RichTextLabel.PropertyName.Text, string.Empty);
    }
}
#endif
