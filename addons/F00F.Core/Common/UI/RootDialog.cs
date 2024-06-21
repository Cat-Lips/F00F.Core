using Godot;

namespace F00F
{
    [Tool]
    public partial class RootDialog : Window
    {
        protected virtual void OnReady() { }

        public sealed override void _Ready()
        {
            InitLayout();
            OnReady();

            void InitLayout()
            {
                SizeChanged += OnSizeChanged;

                void OnSizeChanged()
                {
                    Position = Center() - HalfSize();

                    Vector2I Center() => (Vector2I)this.GetDisplayRect().GetCenter();
                    Vector2I HalfSize() => Size / 2;
                }
            }
        }
    }
}
