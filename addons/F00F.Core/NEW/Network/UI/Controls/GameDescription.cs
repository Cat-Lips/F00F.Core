using Godot;

namespace F00F.NEW;

[Tool]
public partial class GameDescription : StrEdit
{
    protected sealed override void OnReady()
    {
        HideIfEmpty(Text);
        TextChanged += HideIfEmpty;

        void HideIfEmpty(string text)
            => Visible = text.NotEmpty();
    }
}
