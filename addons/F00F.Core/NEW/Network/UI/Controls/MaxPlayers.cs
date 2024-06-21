using Godot;

namespace F00F.NEW;

[Tool]
public partial class MaxPlayers : IntRange
{
    public sealed override void _Ready()
    {
        if (Editor.IsEditor) return;
        OnReady();
    }

    protected virtual void OnReady()
    {
        OnTreeEntered();
        TreeEntered += OnTreeEntered;
        TreeExiting += OnTreeExiting;
        ValueChanged += OnValueChanged;

        void OnTreeEntered()
        {
            SetValue();
            SetMaxValue();
            Network.Instance.MaxAllowedPlayersChanged += SetValue;
            Network.Instance.CurrentProviderChanged += SetMaxValue;
        }

        void OnTreeExiting()
        {
            Network.Instance.MaxAllowedPlayersChanged -= SetValue;
            Network.Instance.CurrentProviderChanged -= SetMaxValue;
        }

        void SetValue()
            => Value = Network.Instance.MaxAllowedPlayers;

        void SetMaxValue()
            => MaxValue = Network.Instance.MaxSupportedPlayers;

        void OnValueChanged(int value)
            => Network.Instance.MaxAllowedPlayers = value;
    }
}
