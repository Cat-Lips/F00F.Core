using Godot;

namespace F00F.NEW;

[Tool]
public partial class PlayerName : StrEdit
{
    protected sealed override void OnReady()
    {
        OnTreeEntered();
        TreeEntered += OnTreeEntered;
        TreeExiting += OnTreeExiting;
        TextChanged += OnTextChanged;

        void OnTreeEntered()
        {
            SetPlayerName();
            Network.Instance.LocalPlayerNameChanged += SetPlayerName;
        }

        void OnTreeExiting()
            => Network.Instance.LocalPlayerNameChanged -= SetPlayerName;

        void SetPlayerName()
            => Text = Network.Instance.LocalPlayerName;

        void OnTextChanged(string text)
            => Network.Instance.LocalPlayerName = text;
    }
}
