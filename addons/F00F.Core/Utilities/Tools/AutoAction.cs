using System;
using Godot;

namespace F00F
{
    public partial class AutoAction : GodotObject
    {
        public event Action Action;
        public bool Triggered { get; private set; }

        public void Run()
        {
            if (Triggered) return;
            if (Action is null) return;

            Triggered = true;
            this.CallDeferred(() =>
            {
                Triggered = false;
                Action?.Invoke();
            });
        }
    }
}
