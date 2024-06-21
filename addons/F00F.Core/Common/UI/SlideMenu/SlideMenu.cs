using System.Linq;
using Godot;

namespace F00F;

[Tool]
public partial class SlideMenu : Root
{
    private Control Label => field ??= GetNode<Label>("%Label");
    protected Control Items => field ??= GetNode<Control>("%Items");

    protected virtual void OnReady1() { }
    protected sealed override void OnReady()
    {
        Editor.Disable(this);

        Label.CustomMinimumSize = CustomMinSize();
        if (!Editor.IsEditor) Items.Visible = this.MouseOver();

        OnReady1();

        Vector2 CustomMinSize()
        {
            var siblings = GetParent().GetChildren<SlideMenu>();
            var x = siblings.Max(x => x.Label.Size.X);
            var y = siblings.Max(x => x.Items.Size.Y);
            return new(x, y);
        }
    }

    public sealed override void _Input(InputEvent @event)
        => Items.Visible = this.MouseOver();
}
