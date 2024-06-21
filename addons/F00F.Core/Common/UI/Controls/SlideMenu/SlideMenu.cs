using System.Linq;
using Godot;

namespace F00F;

[Tool]
public partial class SlideMenu : Root
{
    private Control Label => field ??= (Label)GetNode("%Label");
    protected Control Items => field ??= (Control)GetNode("%Items");

    protected virtual void OnReady1() { }
    protected sealed override void OnReady()
    {
        OnReady();
        OnReady1();

        void OnReady()
        {
            InitMinSize();
            InitVisibility();

            void InitMinSize()
            {
                Label.CustomMinimumSize = CustomMinSize();

                Vector2 CustomMinSize()
                {
                    var siblings = GetParent().GetChildren<SlideMenu>();
                    var x = siblings.Max(x => x.Label.Size.X);
                    var y = siblings.Max(x => x.Items.Size.Y);
                    return new(x, y);
                }
            }

            void InitVisibility()
            {
                if (Editor.IsEditor) return;

                SetVisibility();
                MouseOverChanged += SetVisibility;

                void SetVisibility()
                    => Items.Visible = MouseOver;
            }
        }
    }
}
