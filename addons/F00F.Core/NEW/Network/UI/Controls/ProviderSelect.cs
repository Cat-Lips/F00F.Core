using Godot;

namespace F00F.NEW;

[Tool]
public partial class ProviderSelect : KeyEdit
{
    public sealed override void _Ready()
    {
        if (Editor.IsEditor) return;
        OnReady();
    }

    protected virtual void OnReady()
    {
        InitKeys();
        OnTreeEntered();
        TreeEntered += OnTreeEntered;
        TreeExiting += OnTreeExiting;
        ItemSelected += OnItemSelected;

        void InitKeys()
            => Keys = Network.Instance.Providers;

        void OnTreeEntered()
        {
            SetSelectedItem();
            Network.Instance.ProviderChanged += SetSelectedItem;
        }

        void OnTreeExiting()
            => Network.Instance.ProviderChanged -= SetSelectedItem;

        void SetSelectedItem()
            => Selected = Network.Instance.Provider;

        void OnItemSelected(string value)
            => Network.Instance.Provider = value;
    }
}
