using Godot;

namespace F00F.NEW;

[Tool]
public partial class SessionName : StrEdit
{
    protected sealed override void OnReady()
    {
        OnTreeEntered();
        TreeEntered += OnTreeEntered;
        TreeExiting += OnTreeExiting;
        TextChanged += OnTextChanged;

        void OnTreeEntered()
        {
            SetSessionName();
            Network.Instance.SessionNameChanged += SetSessionName;
        }

        void OnTreeExiting()
            => Network.Instance.SessionNameChanged -= SetSessionName;

        void SetSessionName()
            => Text = Network.Instance.SessionName;

        void OnTextChanged(string text)
            => Network.Instance.SessionName = text;
    }
}
