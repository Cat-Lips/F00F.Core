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
        SetMinValue();
        OnTreeEntered();
        TreeEntered += OnTreeEntered;
        TreeExiting += OnTreeExiting;
        ValueChanged += OnValueChanged;

        void OnTreeEntered()
        {
            SetMaxValue(); SetValue();
            Network.Instance.ProviderChanged += SetMaxValue;
            Network.Instance.MaxPlayersChanged += SetValue;
        }

        void OnTreeExiting()
        {
            Network.Instance.ProviderChanged -= SetMaxValue;
            Network.Instance.MaxPlayersChanged -= SetValue;
        }

        void SetMinValue()
            => MinValue = Network.MinPlayerLimit;

        void SetMaxValue()
            => MaxValue = Network.Instance.MaxSupportedPlayers;

        void SetValue()
            => Value = Network.Instance.MaxPlayers;

        void OnValueChanged(int value)
            => Network.Instance.MaxPlayers = value;
    }
}
