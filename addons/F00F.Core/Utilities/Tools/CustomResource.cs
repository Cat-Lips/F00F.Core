using System;
using Godot;

namespace F00F;

[Tool]
public abstract partial class CustomResource : Resource
{
    private readonly AutoAction ChangedEventHandler = new();

    public new event Action Changed;
    public bool HasChanged => ChangedEventHandler.Triggered;
    public void TriggerChange() => ChangedEventHandler.Run();
    public void InvokeChanged() => ChangedEventHandler.RunNow();

    public void EnableChangedEvent() => this.SafeConnect(Resource.SignalName.Changed, ChangedEventHandler.Run);
    public void DisableChangedEvent() => this.SafeDisconnect(Resource.SignalName.Changed, ChangedEventHandler.Run);

    public CustomResource()
    {
        ChangedEventHandler.Action += () => Changed?.Invoke();

        EnableChangedEvent();
    }
}
