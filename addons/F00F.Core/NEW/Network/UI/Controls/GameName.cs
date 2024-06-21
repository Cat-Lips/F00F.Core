using Godot;

namespace F00F.NEW;

[Tool]
public partial class GameName : StrEdit
{
    protected sealed override void OnReady()
    {
        OnTreeEntered();
        TreeEntered += OnTreeEntered;
        TreeExiting += OnTreeExiting;
        TextChanged += OnTextChanged;

        void OnTreeEntered()
        {
            SetGameName();
            Network.Instance.CurrentGameNameChanged += SetGameName;
        }

        void OnTreeExiting()
            => Network.Instance.CurrentGameNameChanged -= SetGameName;

        void SetGameName()
            => Text = Network.Instance.CurrentGameName;

        void OnTextChanged(string text)
            => Network.Instance.CurrentGameName = text;
    }
}
