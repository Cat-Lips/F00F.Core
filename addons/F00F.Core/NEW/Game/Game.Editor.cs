#if TOOLS
namespace F00F.NEW;

public partial class Game
{
    protected virtual void OnEditorSave()
    {
        Editor.DoPreSaveResetField(this, PropertyName.Input);
        Editor.DoPreSaveResetField(this, PropertyName.Config);
    }
}
#endif
