using System;
using Godot;

namespace F00F
{
    [Tool]
    public partial class GlbEditWindow : Window
    {
        private GlbEdit GlbEdit => GetNode<GlbEdit>("GlbEdit");

        #region Instantiate

        private static readonly PackedScene _scene = Utils.LoadScene<GlbEditWindow>();
        public static GlbEditWindow Instantiate(Node parent, Node source, Action<Node> accept)
        {
            var x = _scene.Instantiate<GlbEditWindow>();
            x.Initialise(parent, source, accept);
            return x;
        }

        #endregion

        private void Initialise(Node parent, Node source, Action<Node> accept)
        {
            Ready += OnReady;
            CloseRequested += CloseWindow;

            PopupExclusiveCenteredClamped(parent, (Vector2I)parent.GetDisplayRect().Size);

            void OnReady()
            {
                Title = $"{source.Name} Preview";
                GlbEdit.EditScene(source, OnEditComplete);

                void OnEditComplete(Node scene)
                {
                    if (scene is not null)
                        accept(scene);

                    CloseWindow();
                }
            }

            void CloseWindow()
                => QueueFree();

        }
    }
}
