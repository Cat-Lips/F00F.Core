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
            Network.Instance.PlayerNameChanged += SetPlayerName;
        }

        void OnTreeExiting()
            => Network.Instance.PlayerNameChanged -= SetPlayerName;

        void SetPlayerName()
            => Text = Network.Instance.PlayerName;

        void OnTextChanged(string text)
            => Network.Instance.PlayerName = text;
    }
}
