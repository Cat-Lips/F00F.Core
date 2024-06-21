using System;
using Godot;

namespace F00F
{
    [Tool]
    public abstract partial class CustomResource : Resource
    {
        private readonly AutoAction ChangedEventHandler = new();

        public new event Action Changed;
        public bool HasChanged => ChangedEventHandler.Triggered;
        public void TriggerChange() => ChangedEventHandler.Run();

        public CustomResource()
        {
            base.Changed += ChangedEventHandler.Run;
            ChangedEventHandler.Action += () => Changed?.Invoke();
        }
    }
}
