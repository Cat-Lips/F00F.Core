using Godot;

namespace F00F.NEW;

[Tool]
public partial class ActiveProviders : KeyEdit
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
        ItemSelected += OnItemSelected;

        void OnTreeEntered()
        {
            SetActiveItems();
            SetSelectedItem();
            Network.Instance.ActiveProvidersChanged += SetActiveItems;
            Network.Instance.CurrentProviderChanged += SetSelectedItem;
        }

        void OnTreeExiting()
        {
            Network.Instance.ActiveProvidersChanged -= SetActiveItems;
            Network.Instance.CurrentProviderChanged -= SetSelectedItem;
        }

        void SetActiveItems()
            => Keys = Network.Instance.ActiveProviders;

        void SetSelectedItem()
            => Selected = Network.Instance.CurrentProvider;

        void OnItemSelected(string value)
            => Network.Instance.CurrentProvider = value;
    }
}
