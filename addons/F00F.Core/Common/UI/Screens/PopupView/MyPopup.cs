using Godot;

namespace F00F;

[Tool]
public partial class MyPopup : PopupPanel
{
    public sealed override void _Ready()
    {
        OnTreeEntered();
        TreeEntered += OnTreeEntered;
        TreeExiting += OnTreeExiting;

        void OnTreeEntered()
        {
            MoveToCenter();
            SizeChanged += MoveToCenter;
        }

        void OnTreeExiting()
            => SizeChanged -= MoveToCenter;
    }
}
